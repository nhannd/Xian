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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using System.ServiceModel;
using System.Security.Permissions;

namespace ClearCanvas.Ris.Application.Services.Admin.UserAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IUserAdminService))]
    public class UserAdminService : ApplicationServiceBase, IUserAdminService
    {
        #region IUserAdminService Members

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role=ClearCanvas.Ris.Application.Common.AuthorityTokens.UserAdmin)]
        public ListUsersResponse ListUsers(ListUsersRequest request)
        {
            UserSearchCriteria criteria = new UserSearchCriteria();

            UserAssembler assembler = new UserAssembler();
            List<UserSummary> userSummaries = CollectionUtils.Map<User, UserSummary, List<UserSummary>>(
                PersistenceContext.GetBroker<IUserBroker>().Find(criteria, request.Page),
                delegate(User user)
                {
                    return assembler.GetUserSummary(user);
                });
            return new ListUsersResponse(userSummaries);
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.UserAdmin)]
        public LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request)
        {
            User user = PersistenceContext.Load<User>(request.UserRef);
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
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.AuthorityGroupAdmin)]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.UserAdmin)]
        public ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request)
        {
            AuthorityGroupSearchCriteria criteria = new AuthorityGroupSearchCriteria();

            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            List<AuthorityGroupSummary> authorityGroups = CollectionUtils.Map<AuthorityGroup, AuthorityGroupSummary, List<AuthorityGroupSummary>>(
                PersistenceContext.GetBroker<IAuthorityGroupBroker>().Find(criteria, request.Page),
                delegate(AuthorityGroup authorityGroup)
                {
                    return assembler.GetAuthorityGroupSummary(authorityGroup);
                });
            return new ListAuthorityGroupsResponse(authorityGroups);
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.AuthorityGroupAdmin)]
        public LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request)
        {
            AuthorityGroup authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupRef);
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            return new LoadAuthorityGroupForEditResponse(authorityGroup.GetRef(), assembler.GetAuthorityGroupDetail(authorityGroup));
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.AuthorityGroupAdmin)]
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
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.UserAdmin)]
        public AddUserResponse AddUser(AddUserRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserDetail, "UserDetail");

            UserDetail userDetail = request.UserDetail;
            User user = User.CreateNewUser(
                new UserInfo(userDetail.UserName, userDetail.DisplayName, userDetail.ValidFrom, userDetail.ValidUntil));

            // copy other info such as authority groups
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
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.UserAdmin)]
        public UpdateUserResponse UpdateUser(UpdateUserRequest request)
        {
            User user = PersistenceContext.Load<User>(request.UserRef);

            // update user account info
            UserAssembler assembler = new UserAssembler();
            assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

            // reset password if requested
            if (request.UserDetail.ResetPassword)
                user.ResetPassword();

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
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.UserAdmin)]
        public ResetUserPasswordResponse ResetUserPassword(ResetUserPasswordRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserRef, "UserRef");

            User user = PersistenceContext.Load<User>(request.UserRef);
            user.ResetPassword();

            UserAssembler assembler = new UserAssembler();
            return new ResetUserPasswordResponse(assembler.GetUserSummary(user));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.AuthorityGroupAdmin)]
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
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.AuthorityGroupAdmin)]
        public UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request)
        {
            AuthorityGroup authorityGroup = PersistenceContext.Load<AuthorityGroup>(request.AuthorityGroupRef);
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            assembler.UpdateAuthorityGroup(authorityGroup, request.AuthorityGroupDetail, PersistenceContext);

            PersistenceContext.Lock(authorityGroup, DirtyState.Dirty);

            return new UpdateAuthorityGroupResponse(assembler.GetAuthorityGroupSummary(authorityGroup));
        }

        #endregion

    }
}
