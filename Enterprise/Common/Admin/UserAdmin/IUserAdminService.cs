#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
        /// Load details for a specified user account.
        /// </summary>
        /// <param name="request"><see cref="LoadUserForEditRequest"/></param>
        /// <returns><see cref="LoadUserForEditResponse"/></returns>
        [OperationContract]
        LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request);

		/// <summary>
		/// List active sessions for a specified user account.
		/// </summary>
		/// <param name="request"><see cref="ListUserSessionsRequest"/></param>
		/// <returns><see cref="ListUserSessionsResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		ListUserSessionsResponse ListUserSessions(ListUserSessionsRequest request);

		/// <summary>
		/// Terminate the specified session(s).
		/// </summary>
		/// <param name="request"><see cref="ListUserSessionsRequest"/></param>
		/// <returns><see cref="TerminateUserSessionResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		TerminateUserSessionResponse TerminateUserSession(TerminateUserSessionRequest request);
	}
}
