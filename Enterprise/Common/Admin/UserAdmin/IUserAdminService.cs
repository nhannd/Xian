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

using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
    /// <summary>
    /// Provides operations to administer user accounts and authority groups
    /// </summary>
    [EnterpriseCoreService]
    [ServiceContract]
    public interface IUserAdminService
    {
        /// <summary>
        /// Summary list of all user accounts
        /// </summary>
        /// <param name="request"><see cref="ListUsersRequest"/></param>
        /// <returns><see cref="ListUsersResponse"/></returns>
        [OperationContract]
        ListUsersResponse ListUsers(ListUsersRequest request);

        /// <summary>
        /// Add a new user account
        /// </summary>
        /// <param name="request"><see cref="AddUserRequest"/></param>
        /// <returns><see cref="AddUserResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddUserResponse AddUser(AddUserRequest request);

        /// <summary>
        /// Updates a user account.  The UserID cannot be updated
        /// </summary>
        /// <param name="request"><see cref="UpdateUserRequest"/></param>
        /// <returns><see cref="UpdateUserResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateUserResponse UpdateUser(UpdateUserRequest request);

		/// <summary>
		/// Deletes a user account.
		/// </summary>
		/// <param name="request"><see cref="DeleteUserRequest"/></param>
		/// <returns><see cref="DeleteUserResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteUserResponse DeleteUser(DeleteUserRequest request);

        /// <summary>
        /// Resets a user's password to the temporary password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        ResetUserPasswordResponse ResetUserPassword(ResetUserPasswordRequest request);

        /// <summary>
        /// Load details for a specified user account
        /// </summary>
        /// <param name="request"><see cref="LoadUserForEditRequest"/></param>
        /// <returns><see cref="LoadUserForEditResponse"/></returns>
        [OperationContract]
        LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request);
	}
}
