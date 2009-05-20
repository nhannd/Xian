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
using System.Management;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Login;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Holds information related to the current login session.
    /// </summary>
    public sealed class LoginSession
    {
        private static LoginSession _current;

        /// <summary>
        /// Gets the current <see cref="LoginSession"/>.
        /// </summary>
        public static LoginSession Current
        {
            get { return _current; }
        }

        public static void Create(string userName, string password)
        {
            Create(userName, password, null);
        }

        /// <summary>
        /// Creates a new <see cref="LoginSession"/>.
        /// </summary>
        /// <remarks>
        /// Contacts the server and requests login using the specified credentials.  An exception will be thrown
        /// if the credentials are not valid.
        /// </remarks>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="facility"></param>
        public static void Create(string userName, string password, FacilitySummary facility)
        {
            try
            {
                Platform.Log(LogLevel.Debug, "Attempting login...");

                Platform.GetService<ILoginService>(
                    delegate(ILoginService service)
                    {
                        LoginResponse response = service.Login(
							new LoginRequest(
								userName,
								password,
								facility == null ? null : facility.FacilityRef,
								Dns.GetHostName(),
								GetIPAddress(),
								GetMachineID()));

                        // if the call succeeded, set a default principal object on this thread, containing
                        // the set of authority tokens for this user
                        Thread.CurrentPrincipal = DefaultPrincipal.CreatePrincipal(
                            new GenericIdentity(userName),
                            response.SessionToken,
                            response.UserAuthorityTokens);

                        // set the current session before attempting to access other services, as these will require authentication
                        _current = new LoginSession(userName, response.SessionToken, response.StaffSummary, facility);
                    });

                Platform.Log(LogLevel.Debug, "Login attempt was successful.");
            }
            catch (FaultException<UserAccessDeniedException> e)
            {
                Platform.Log(LogLevel.Debug, e.Detail, "Login attempt failed.");
                throw e.Detail;
            }
            catch (FaultException<PasswordExpiredException> e)
            {
                Platform.Log(LogLevel.Debug, e.Detail, "Login attempt failed.");
                throw e.Detail;
            }
        }

        internal static void ChangePassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                Platform.GetService<ILoginService>(
                    delegate(ILoginService service)
                    {
                        service.ChangePassword(new ChangePasswordRequest(userName, oldPassword, newPassword, GetIPAddress(), GetMachineID()));
                    });

            }
			catch (FaultException<UserAccessDeniedException> e)
            {
                throw e.Detail;
            }
            catch (FaultException<RequestValidationException> e)
            {
                throw e.Detail;
            }
        }

        private readonly string _userName;
        private readonly SessionToken _sessionToken;
        private readonly StaffSummary _staff;
        private readonly FacilitySummary _workingFacility;

        private LoginSession(string userName, SessionToken sessionToken, StaffSummary staff, FacilitySummary workingFacility)
        {
            _userName = userName;
            _sessionToken = sessionToken;
            _staff = staff;
            _workingFacility = workingFacility;
        }

        /// <summary>
        /// Terminates the current login session, setting the <see cref="Current"/> property to null.
        /// </summary>
        public void Terminate()
        {
            try
            {
                Platform.GetService<ILoginService>(
                    delegate(ILoginService service)
                    {
						service.Logout(new LogoutRequest(_userName, _sessionToken, GetIPAddress(), GetMachineID()));
                    });
            }
            finally
            {
                _current = null;
            }
        }

        /// <summary>
        /// Gets the user name of the logged on user.
        /// </summary>
        public string UserName
        {
            get { return _userName; }
        }

        /// <summary>
        /// Gets the full person name of the logged on user.
        /// </summary>
        public PersonNameDetail FullName
        {
            get { return _staff == null ? null : _staff.Name; }
        }

        /// <summary>
        /// Gets the <see cref="StaffSummary"/> of the logged on user.
        /// </summary>
        public StaffSummary Staff
        {
            get { return _staff; }
        }

        /// <summary>
        /// Gets if the user is associated with a RIS staff person.
        /// </summary>
        public bool IsStaff
        {
            get { return _staff != null; }
        }

        /// <summary>
        /// Gets the current working facility.
        /// </summary>
        public FacilitySummary WorkingFacility
        {
            get { return _workingFacility; }
        }

        /// <summary>
        /// Gets the session token.  This property is internal in order to limit exposure of the session
        /// token.
        /// </summary>
        internal string SessionToken
        {
            get { return _sessionToken.Id; }
        }

        /// <summary>
        /// Utility method to get the local IP address to report to the server.
        /// </summary>
        /// <returns></returns>
        private static string GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            // just use the first address
            // we don't care very much because this is just for auditing purposes, 
            // it serves no technical purpose
            return addresses.Length > 0 ? addresses[0].ToString() : null;
        }

        private static string GetMachineID()
        {
            try
            {
				// Use the serial number of the mother board
                string id = null;
                ManagementClass mc = new ManagementClass("Win32_Baseboard");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
					id = mo.Properties["SerialNumber"].Value.ToString().Trim();
					if (!string.IsNullOrEmpty(id))
                        break;
                }

				return id;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e);
                return null;
            }
        }
    }
}
