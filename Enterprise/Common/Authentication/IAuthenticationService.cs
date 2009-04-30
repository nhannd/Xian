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
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	/// <summary>
	/// Provides methods for validating user credentials and obtaining the set of authority tokens granted to a user.
	/// </summary>
	[EnterpriseCoreService]
	[ServiceContract]
	[Authentication(false)]
	public interface IAuthenticationService
	{
		/// <summary>
		/// Initiates a new session for the specified user, first verifying the password,
		/// and returns a new session token if successful.
		/// </summary>
		/// <exception cref="SecurityTokenException">Invalid username or password.</exception>
		/// <exception cref="PasswordExpiredException">Password is valid but has expired.</exception>
		[OperationContract]
		[FaultContract(typeof(PasswordExpiredException))]
		InitiateSessionResponse InitiateSession(InitiateSessionRequest request);

		/// <summary>
		/// Validates an existing user session, returning an updated session token.
		/// </summary>
		/// <exception cref="SecurityTokenException">Session token expired or otherwise invalid.</exception>
		[OperationContract]
		ValidateSessionResponse ValidateSession(ValidateSessionRequest request);

		/// <summary>
		/// Terminates an existing user session.
		/// </summary>
		/// <exception cref="SecurityTokenException">Session token expired or otherwise invalid.</exception>
		[OperationContract]
		TerminateSessionResponse TerminateSession(TerminateSessionRequest request);

		/// <summary>
		/// Changes the password for the specified user account.
		/// </summary>
		/// <exception cref="SecurityTokenException">Session token expired or otherwise invalid.</exception>
		/// <exception cref="RequestValidationException">The new password does not meet password policy restrictions.</exception>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		ChangePasswordResponse ChangePassword(ChangePasswordRequest request);

		/// <summary>
		/// Obtains the set of authority tokens that have been granted to the 
		/// specified user.  Note: we may want to remove this method and use ValidateSession instead,
		/// since that method also returns the current list of authorizations
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		GetAuthorizationsResponse GetAuthorizations(GetAuthorizationsRequest request);
	}
}