using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.AuthenticationAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IAuthenticationAdminService))]
    class AuthenticationAdminService : ApplicationServiceBase, IAuthenticationAdminService
    {
        #region IAuthorityAdminService Members

        [ReadOperation]
        public ListUsersResponse ListUsers(ListUsersRequest request)
        {
            // ignore request

            UserAssembler assembler = new UserAssembler();
            List<UserSummary> userSummaries = CollectionUtils.Map<User, UserSummary, List<UserSummary>>(
                PersistenceContext.GetBroker<IUserBroker>().FindAll(),
                delegate(User user)
                {
                    return assembler.GetUserSummary(user);
                });
            return new ListUsersResponse(userSummaries);
        }

        [ReadOperation]
        public LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request)
        {
            User user = (User)PersistenceContext.Load(request.UserRef);
            UserAssembler assembler = new UserAssembler();
            return new LoadUserForEditResponse(user.GetRef(), assembler.GetUserDetail(user));
        }

        [ReadOperation]
        public ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request)
        {
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            List<AuthorityGroupSummary> authorityGroups = CollectionUtils.Map<AuthorityGroup, AuthorityGroupSummary, List<AuthorityGroupSummary>>(
                PersistenceContext.GetBroker<IAuthorityGroupBroker>().FindAll(),
                delegate(AuthorityGroup authorityGroup)
                {
                    return assembler.GetAuthorityGroupSummary(authorityGroup);
                });
            return new ListAuthorityGroupsResponse(authorityGroups);
        }

        [ReadOperation]
        public LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request)
        {
            AuthorityGroup authorityGroup = (AuthorityGroup)PersistenceContext.Load(request.AuthorityGroupRef);
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            return new LoadAuthorityGroupForEditResponse(authorityGroup.GetRef(), assembler.GetAuthorityGroupDetail(authorityGroup));
        }

        [ReadOperation]
        public ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request)
        {
            AuthorityTokenAssembler assembler = new AuthorityTokenAssembler();
            List<AuthorityTokenSummary> authorityTokens = CollectionUtils.Map<AuthorityToken, AuthorityTokenSummary, List<AuthorityTokenSummary>>(
                PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindAll(),
                delegate(AuthorityToken authorityToken)
                {
                    return assembler.GetAuthorityTokenSummary(authorityToken);
                });
            return new ListAuthorityTokensResponse(authorityTokens);
        }

        [UpdateOperation]
        public AddUserResponse AddUser(AddUserRequest request)
        {
            User user = new User();
            UserAssembler assembler = new UserAssembler();
            assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

            PersistenceContext.Lock(user, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddUserResponse(user.GetRef(), assembler.GetUserSummary(user));
        }

        [UpdateOperation]
        public UpdateUserResponse UpdateUser(UpdateUserRequest request)
        {
            User user = (User)PersistenceContext.Load(request.UserRef);
            UserAssembler assembler = new UserAssembler();
            assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

            PersistenceContext.Lock(user, DirtyState.Dirty);

            return new UpdateUserResponse(assembler.GetUserSummary(user));
        }

        [UpdateOperation]
        public AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request)
        {
            AuthorityGroup authorityGroup = new AuthorityGroup();
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

            PersistenceContext.Lock(authorityGroup, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddAuthorityGroupResponse(assembler.GetAuthorityGroupSummary(authorityGroup));
        }

        [UpdateOperation]
        public UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request)
        {
            AuthorityGroup authorityGroup = (AuthorityGroup)PersistenceContext.Load(request.AuthorityGroupRef);
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

            PersistenceContext.Lock(authorityGroup, DirtyState.Dirty);

            return new UpdateAuthorityGroupResponse(assembler.GetAuthorityGroupSummary(authorityGroup));
        }

        #endregion
    }
}
