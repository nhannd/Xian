#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Security;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Enterprise.Authentication
{
    /// <summary>
    /// Wrapper for <see cref="IAuthenticationService"/> service.
    /// </summary>
    public sealed class LoginService : IDisposable
    {
        public SessionInfo Login(string userName, string password)
        {
            SessionInfo session = null;
            
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService  service)
                    {
                        try
                        {
                            InitiateSessionRequest request = new InitiateSessionRequest(userName, password);
                            request.GetAuthorizations = true;
                            InitiateSessionResponse response = service.InitiateSession(request);

                            if (response != null)
                            {
                                LoginCredentials credentials = new LoginCredentials();
                                credentials.UserName = userName;
                                credentials.DisplayName = response.DisplayName;
                                credentials.SessionToken = response.SessionToken;
                                credentials.Authorities = response.AuthorityTokens;
                                CustomPrincipal user = new CustomPrincipal(new CustomIdentity(userName, response.DisplayName),credentials);
                                session = new SessionInfo(user);

                                Platform.Log(LogLevel.Info, "{0} has successfully logged in.", userName);                                
                            }
                        }
                        catch (FaultException<PasswordExpiredException> ex)
                        {
                            throw ex.Detail;
                        }
                    }
                );

            return session;
        }

        public void Logout(SessionInfo session)
        {
            TerminateSessionRequest request =
                new TerminateSessionRequest(session.User.Identity.Name, session.Credentials.SessionToken);

            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                    {
                        service.TerminateSession(request);
                    });
        }

        public void Validate(SessionInfo session)
        {
            ValidateSessionRequest request = new ValidateSessionRequest(session.User.Identity.Name, session.Credentials.SessionToken);
            request.GetAuthorizations = true;

            try
            {
                Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                    {
                        ValidateSessionResponse response = service.ValidateSession(request);
                        // update session info
                        session.Credentials.Authorities = response.AuthorityTokens;
                        session.Credentials.SessionToken = response.SessionToken;
                    });
            }
            catch(Exception ex)
            {
                //TODO: for now we can't distinguish communicate errors and credential validation errors.
                // All exceptions are treated the same: we can't verify the login.
                SessionValidationException e = new SessionValidationException(ex);
                throw e;
            }
            

            
        }

        public void ChangePassword(string userName, string oldPassword, string newPassword)
        {

            ChangePasswordRequest request = new ChangePasswordRequest(userName, oldPassword, newPassword);
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                    {
                        service.ChangePassword(request);
                        Platform.Log(LogLevel.Info, "Password for {0} has been changed.", userName);
                    });
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}