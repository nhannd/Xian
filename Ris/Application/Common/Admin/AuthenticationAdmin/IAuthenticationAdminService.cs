using System;

using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [ServiceContract]
    public interface IAuthenticationAdminService
    {
        [OperationContract]
        ListUsersResponse ListUsers(ListUsersRequest request);

        [OperationContract]
        AddUserResponse AddUser(AddUserRequest request);

        [OperationContract]
        UpdateUserResponse UpdateUser(UpdateUserRequest request);

        [OperationContract]
        LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request);

        [OperationContract]
        ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request);

        [OperationContract]
        AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request);

        [OperationContract]
        UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request);

        [OperationContract]
        LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request);

        [OperationContract]
        ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request);
    }
}
