using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using System.ServiceModel;

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
            UserSearchCriteria criteria = new UserSearchCriteria();
            SearchResultPage page = new SearchResultPage(request.PageRequest.FirstRow, request.PageRequest.MaxRows);

            UserAssembler assembler = new UserAssembler();
            List<UserSummary> userSummaries = CollectionUtils.Map<User, UserSummary, List<UserSummary>>(
                PersistenceContext.GetBroker<IUserBroker>().Find(criteria, page),
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
            Staff staff = null;
            try
            {
                IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
                staff = staffBroker.FindStaffForUser(user.UserName);
            }
            catch (EntityNotFoundException)
            {
                // no staff for user
            }

            UserAssembler assembler = new UserAssembler();
            return new LoadUserForEditResponse(user.GetRef(), assembler.GetUserDetail(user, staff));
        }

        [ReadOperation]
        public ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request)
        {
            AuthorityGroupSearchCriteria criteria = new AuthorityGroupSearchCriteria();
            SearchResultPage page = new SearchResultPage(request.PageRequest.FirstRow, request.PageRequest.MaxRows);

            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            List<AuthorityGroupSummary> authorityGroups = CollectionUtils.Map<AuthorityGroup, AuthorityGroupSummary, List<AuthorityGroupSummary>>(
                PersistenceContext.GetBroker<IAuthorityGroupBroker>().Find(criteria, page),
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
        [FaultContract(typeof(RequestValidationException))]
        public AddUserResponse AddUser(AddUserRequest request)
        {
            if (UserIdExists(request.UserDetail.UserId))
            {
                throw new RequestValidationException(string.Format(SR.ExceptionUserIDAlreadyExists, request.UserDetail.UserId));
            }

            User user = new User();
            UserAssembler assembler = new UserAssembler();
            assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

            PersistenceContext.Lock(user, DirtyState.New);

            // create staff association
            if (request.UserDetail.StaffRef != null)
            {
                IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
                Staff staff = staffBroker.Load(request.UserDetail.StaffRef);
                staff.User = user;
            }

            PersistenceContext.SynchState();

            return new AddUserResponse(user.GetRef(), assembler.GetUserSummary(user));
        }

        [UpdateOperation]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        public UpdateUserResponse UpdateUser(UpdateUserRequest request)
        {
            User user = (User)PersistenceContext.Load(request.UserRef);

            if (user.UserName != request.UserDetail.UserId
                && UserIdExists(request.UserDetail.UserId))
            {
                throw new RequestValidationException(string.Format(SR.ExceptionUserIDAlreadyExists, request.UserDetail.UserId));
            }

            UserAssembler assembler = new UserAssembler();
            assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

            IStaffBroker staffBroker = PersistenceContext.GetBroker<IStaffBroker>();
            try
            {
                Staff staff = staffBroker.FindStaffForUser(user.UserName);
                staff.User = null;  // dissociate any previously associated staff
            }
            catch (EntityNotFoundException)
            {
                // no previously associated staff
            }

            // create staff association
            if (request.UserDetail.StaffRef != null)
            {
                Staff staff = staffBroker.Load(request.UserDetail.StaffRef);
                staff.User = user;
            }

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
        [FaultContract(typeof(ConcurrentModificationException))]
        public UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request)
        {
            AuthorityGroup authorityGroup = (AuthorityGroup)PersistenceContext.Load(request.AuthorityGroupRef);
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

            PersistenceContext.Lock(authorityGroup, DirtyState.Dirty);

            return new UpdateAuthorityGroupResponse(assembler.GetAuthorityGroupSummary(authorityGroup));
        }

        #endregion

        private bool UserIdExists(string userId)
        {
            UserSearchCriteria criteria = new UserSearchCriteria();
            criteria.UserName.EqualTo(userId);

            IUserBroker broker = PersistenceContext.GetBroker<IUserBroker>();
            return broker.Count(criteria) > 0;
        }
    }
}
