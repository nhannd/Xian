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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Enterprise.Authentication
{

    public class SessionInfo
    {
        private readonly CustomPrincipal _user;
        private bool _valid = false;

        public SessionInfo(CustomPrincipal user)
        {
            _user = user;

            Validate(); // this would refresh the authority groups
        }

        public SessionInfo(string loginId, string name, SessionToken token)
            : this(new CustomPrincipal(new CustomIdentity(loginId, name),
                                       CreateLoginCredentials(loginId, name, token)))
        {

        }

        /// <summary>
        /// Gets a value indicating whether or not the session information is valid.
        /// </summary>
        public bool Valid
        {
            get
            {
                return _valid;
            }
        }

        public CustomPrincipal User
        {
            get { return _user; }
        }

        public LoginCredentials Credentials
        {
            get { return _user.Credentials; }
        }

        private static LoginCredentials CreateLoginCredentials(string loginId, string name, SessionToken token)
        {
            LoginCredentials credentials = new LoginCredentials();
            credentials.UserName = loginId;
            credentials.DisplayName = name;
            credentials.SessionToken = token;
            return credentials;
        }

        private void Validate()
        {
            _valid = false;
            using(LoginService service = new LoginService())
            {
                service.Validate(this);
                _valid = true;
            }   
        }
    }
}