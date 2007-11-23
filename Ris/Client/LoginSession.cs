#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Login;
using System.Threading;
using System.Security.Principal;
using ClearCanvas.Ris.Application.Common;

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
        internal static void Create(string userName, string password, FacilitySummary facility)
        {
            Platform.GetService<ILoginService>(
                delegate(ILoginService service)
                {
                    LoginResponse response = service.Login(new LoginRequest(userName, password, facility.FacilityRef));

                    // if the call succeeded, construct a generic principal object on this thread, containing
                    // the set of authority tokens for this user
                    Thread.CurrentPrincipal = new GenericPrincipal(
                        new GenericIdentity(userName), response.UserAuthorityTokens);

                    // set the current session before attempting to access other services, as these will require authentication
                    _current = new LoginSession(userName, password, response.FullName, facility);
                });
        }

        private readonly string _userName;
        private readonly string _sessionToken;
        private readonly PersonNameDetail _fullName;
        private readonly FacilitySummary _workingFacility;

        private LoginSession(string userName, string sessionToken, PersonNameDetail fullName, FacilitySummary workingFacility)
        {
            _userName = userName;
            _sessionToken = sessionToken;
            _fullName = fullName;
            _workingFacility = workingFacility;
            
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
            get { return _fullName; }
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
            get { return _sessionToken; }
        }

    }
}
