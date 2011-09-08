#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.Web.Enterprise.Authentication
{
    /// <summary>
    /// Wrapper for <see cref="IAuthenticationService"/> service.
    /// </summary>
    public sealed class LoginService : IDisposable
    {
        public SessionInfo Login(string userName, string password, string appName)
        {
            Platform.CheckForEmptyString(userName, "userName");
            Platform.CheckForEmptyString(password, "password");
            Platform.CheckForEmptyString(appName, "appName");

            SessionInfo session = null;
            
            Platform.GetService(
                delegate(IAuthenticationService  service)
                    {
                        try
                        {

                            var request = new InitiateSessionRequest(userName, appName,
                                                                     Dns.GetHostName(), password)
                                              {
                                                  GetAuthorizations = true
                                              };

                            InitiateSessionResponse response = service.InitiateSession(request);
                            if (response != null)
                            {
                                var credentials = new LoginCredentials
                                                      {
                                                          UserName = userName,
                                                          DisplayName = response.DisplayName,
                                                          SessionToken = response.SessionToken,
                                                          Authorities = response.AuthorityTokens,
                                                          DataAccessAuthorityGroups = response.DataGroupOids,
                                                          EmailAddress = response.EmailAddress
                                                      };
                                var user = new CustomPrincipal(new CustomIdentity(userName, response.DisplayName),credentials);
                                Thread.CurrentPrincipal = user;

                                session = new SessionInfo(user);

                                // Note: need to insert into the cache before calling SessionInfo.Validate()
                                SessionCache.Instance.AddSession(response.SessionToken.Id, session);
                                session.Validate();
                                
                                Platform.Log(LogLevel.Info, "{0} has successfully logged in.", userName);                                
                            }                            
                        }
                        catch (FaultException<PasswordExpiredException> ex)
                        {
                            throw ex.Detail;
                        }
                        catch(FaultException<UserAccessDeniedException> ex)
                        {
                            throw ex.Detail;
                        }
                    }
                );

            return session;
        }

        public SessionInfo Query(string id)
        {
            var sessionInfo = SessionCache.Instance.Find(id);
            return sessionInfo;
        }

        public void Logout(string tokenId)
        {
            var session = SessionCache.Instance.Find(tokenId);
            if (session == null)
            {
                throw new Exception(String.Format("Unexpected error: session {0} does not exist in the cache", tokenId));
            }

            var request = new TerminateSessionRequest(session.Credentials.UserName,
                                                      session.Credentials.SessionToken);


            Platform.GetService(
                delegate(IAuthenticationService service)
                    {
                        service.TerminateSession(request);
                        SessionCache.Instance.RemoveSession(tokenId);
                    });
        }

        public SessionToken Renew(string tokenId)
        {
            SessionInfo sessionInfo = SessionCache.Instance.Find(tokenId);
            if (sessionInfo == null)
            {
                throw new Exception(String.Format("Unexpected error: session {0} does not exist in the cache", tokenId));
            }

            var request = new ValidateSessionRequest(sessionInfo.Credentials.UserName,
                                                     sessionInfo.Credentials.SessionToken)
                              {
                                  GetAuthorizations = true
                              };

            try
            {
                SessionToken newToken = null;
                Platform.GetService(
                    delegate(IAuthenticationService service)
                        {
                            DateTime originalExpiryTime = sessionInfo.Credentials.SessionToken.ExpiryTime;
                            ValidateSessionResponse response = service.ValidateSession(request);
                            // update session info
                            string id = response.SessionToken.Id;
                            newToken= SessionCache.Instance.Renew(id, response.SessionToken.ExpiryTime);

                            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                            {
                                Platform.Log(LogLevel.Debug, "Session {0} for {1} is renewed. Valid until {2}", id, sessionInfo.Credentials.UserName, newToken.ExpiryTime);

                                if (originalExpiryTime == newToken.ExpiryTime)
                                {
                                    Platform.Log(LogLevel.Warn, "Session expiry time is not changed. Is it cached?");
                                }
                            }
                        });

                return newToken;
            }
            catch(FaultException<InvalidUserSessionException> ex)
            {
                throw new SessionValidationException(ex.Detail);
            }
            catch(FaultException<UserAccessDeniedException> ex)
            {
                throw new SessionValidationException(ex.Detail);   
            }
            catch(Exception ex)
            {
                //TODO: for now we can't distinguish communicate errors and credential validation errors.
                // All exceptions are treated the same: we can't verify the login.
                var e = new SessionValidationException(ex);
                throw e;
            }            
        }

        public void ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var request = new ChangePasswordRequest(userName, oldPassword, newPassword);
            Platform.GetService(
                delegate(IAuthenticationService service)
                    {
                        service.ChangePassword(request);
                        Platform.Log(LogLevel.Info, "Password for {0} has been changed.", userName);
                    });
        }

        public void ResetPassword(string userName)
        {
            ResetPasswordResponse response;
            var request = new ResetPasswordRequest(userName);
            Platform.GetService(
                delegate(IAuthenticationService service)
                {
                    response = service.ResetPassword(request);
                    Platform.Log(LogLevel.Info, "Password for {0} has been reset and email sent to {1}.", userName, response.EmailAddress);
                });
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

    }

    /// <summary>
    /// Internal session cache. 
    /// </summary>
    public class SessionCache : IDisposable
    {
        private static readonly SessionCache _instance = new SessionCache();
        private static readonly Dictionary<string, SessionInfo> _cacheSessionInfo = new Dictionary<string, SessionInfo>();
        private Timer _timer;
        private readonly object _sync = new object();

        public static SessionCache Instance
        {
            get { return _instance; }
        }

        private SessionCache()
        {
            _timer = new Timer(OnTimer, this, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        private void OnTimer(object state)
        {
            if (_cacheSessionInfo.Count == 0)
                return;

            var list = new List<SessionInfo>(_cacheSessionInfo.Values);
            var active = new StringBuilder();
            active.AppendLine("Active Sessions:");
            var inactive = new StringBuilder();
            inactive.AppendLine("Inactive Sessions:");

            int activeCount = 0;
            int inactiveCount = 0;
            foreach (SessionInfo session in list)
            {
                if (session.Credentials.SessionToken.ExpiryTime < Platform.Time)
                {
                    if (Platform.Time - session.Credentials.SessionToken.ExpiryTime > TimeSpan.FromSeconds(10))
                    {
                        CleanupSession(session);
                        Platform.Log(LogLevel.Debug, "Removed expired idle session: {0} for user {1}",
                                     session.Credentials.SessionToken.Id, session.Credentials.UserName);
                    }
                    else
                    {
                        inactive.AppendLine(String.Format("\t{0}\t{1}: Expired on {2}", session.Credentials.UserName,
                                                          session.Credentials.SessionToken.Id,
                                                          session.Credentials.SessionToken.ExpiryTime));
                        inactiveCount++;
                    }
                }
                else
                {
                    activeCount++;
                    active.AppendLine(String.Format("\t{0}\t{1}: Active. Expiring on {2}", session.Credentials.UserName,
                                                    session.Credentials.SessionToken.Id, session.Credentials.SessionToken.ExpiryTime));
                }
            }
            if (activeCount > 0)
                Platform.Log(LogLevel.Debug, active.ToString());
            if (inactiveCount > 0)
                Platform.Log(LogLevel.Debug, inactive.ToString());
            
        }

        public void AddSession(string id, SessionInfo session)
        {
            lock (_sync)
            {
                _cacheSessionInfo.Add(id, session);
            }

        }

        private void OnSessionRemoved(SessionInfo session)
        {

        }

        private void CleanupSession(SessionInfo session)
        {
            lock (_sync)
            {
                using (var service = new LoginService())
                {
                    try
                    {
                        try
                        {
                            service.Logout(session.Credentials.SessionToken.Id);
                        }
                        catch(Exception ex)
                        {
                            Platform.Log(LogLevel.Warn, ex, "Unable to terminate session {0} gracefully",
                                         session.Credentials.SessionToken.Id);
                        }
                    }
                    finally
                    {
                        RemoveSession(session.Credentials.SessionToken.Id);
                    }
                }
            }
        }

        public void RemoveSession(string id)
        {
            lock (_sync)
            {
                SessionInfo session;
                if (_cacheSessionInfo.TryGetValue(id, out session))
                {
                    _cacheSessionInfo.Remove(id);
                    OnSessionRemoved(session);
                }
            }
        }

        public SessionInfo Find(string id)
        {
            lock (_sync)
            {
                if (_cacheSessionInfo.ContainsKey(id))
                    return _cacheSessionInfo[id];

                return null;
            }
        }

        public SessionToken Renew(string tokenId, DateTime time)
        {
            lock (_sync)
            {
                var sessionInfo = _cacheSessionInfo[tokenId];
                var newToken = new SessionToken(sessionInfo.Credentials.SessionToken.Id, time);
                sessionInfo.Credentials.SessionToken = newToken;

                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Session {0} renewed. Will expire on {1}", sessionInfo.Credentials.SessionToken.Id, sessionInfo.Credentials.SessionToken.ExpiryTime);
                return newToken;
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}