#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.IdentityModel.Tokens;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Provides methods for validating user credentials and obtaining the set of authority tokens granted to a user.
    /// </summary>
    [CoreServiceProvider]
    [ServiceContract]
    public interface IAuthenticationService
    {
        /// <summary>
        /// Initiates a new session for the specified user, first verifying the password,
        /// and returns a new session token if successful.
        /// </summary>
        /// <remarks>
        /// Implementations throw a <see cref="SecurityTokenException"/> if the username
        /// or password is invalid.
        /// </remarks>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException">Invalid username or password.</exception>
        [OperationContract]
        [FaultContract(typeof(PasswordExpiredException))]
        SessionToken InitiateUserSession(string userName, string password);

        /// <summary>
        /// Validates an existing user session, returning a new session token
        /// that has the same identifier but an updated expiry time.
        /// </summary>
        /// <remarks>
		/// Implementations throw a <see cref="SecurityTokenException"> if the session
        /// token has expired or is otherwise invalid.
        /// </remarks>
        /// <param name="userName"></param>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException">Session token expired or otherwise invalid.</exception>
        [OperationContract]
        SessionToken ValidateUserSession(string userName, SessionToken sessionToken);

        /// <summary>
        /// Terminates an existing user session.
        /// </summary>
        /// <remarks>
		/// Implementations throw a <see cref="SecurityTokenException"> if the session
        /// token has expired or is otherwise invalid.
        /// </remarks>
        /// <param name="userName"></param>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        [OperationContract]
        void TerminateUserSession(string userName, SessionToken sessionToken);

        /// <summary>
        /// Changes the password for the specified user account.
        /// </summary>
		/// <remarks>
		/// Implementations throw a <see cref="SecurityTokenException"> if the session
		/// token has expired or is otherwise invalid.
		/// </remarks>
		/// <param name="userName"></param>
        /// <param name="currentPassword"></param>
        /// <param name="newPassword"></param>
        [OperationContract]
        void ChangePassword(string userName, string currentPassword, string newPassword);

        /// <summary>
        /// Obtains the set of authority tokens that have been granted to the 
        /// specified user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
        string[] ListAuthorityTokensForUser(string userName);
    }
}
