using System;

using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    /// <summary>
    /// Provides operations to administer user accounts and authority groups
    /// </summary>
    [ServiceContract]
    public interface IAuthenticationAdminService
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
        /// Load details for a specified user account
        /// </summary>
        /// <param name="request"><see cref="LoadUserForEditRequest"/></param>
        /// <returns><see cref="LoadUserForEditResponse"/></returns>
        [OperationContract]
        LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request);

        /// <summary>
        /// Summary list of all authority groups
        /// </summary>
        /// <param name="request"><see cref="ListAuthorityGroupRequest"/></param>
        /// <returns><see cref="ListAuthorityGroupResponse"/></returns>
        [OperationContract]
        ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request);

        /// <summary>
        /// Add a new authority group
        /// </summary>
        /// <param name="request"><see cref="AddAuthorityGroupRequest"/></param>
        /// <returns><see cref="AddAuthorityGroupResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request);

        /// <summary>
        /// Updates an authority group
        /// </summary>
        /// <param name="request"><see cref="UpdateAuthorityGroupRequest"/></param>
        /// <returns><see cref="UpdateAuthorityGroupResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request);

        /// <summary>
        /// Load details for a specified authority group
        /// </summary>
        /// <param name="request"><see cref="LoadAuthorityGroupForEditRequest"/></param>
        /// <returns><see cref="LoadAuthorityGroupForEditResponse"/></returns>
        [OperationContract]
        LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request);

        /// <summary>
        /// Provides a list of all available authority tokens
        /// </summary>
        /// <param name="request"><see cref="ListAuthorityTokensRequest"/></param>
        /// <returns><see cref="ListAuthorityTokensResponse"/></returns>
        [OperationContract]
        ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request);
    }
}
