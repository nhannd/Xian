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

using System.Collections.Generic;
using System.Security.Permissions;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Core;
using System.Threading;

namespace ClearCanvas.Enterprise.Authentication.Admin.UserAdmin
{
	[ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IUserAdminService))]
    public class UserAdminService : CoreServiceLayer, IUserAdminService
    {
        #region IUserAdminService Members

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
        public ListUsersResponse ListUsers(ListUsersRequest request)
        {
            UserSearchCriteria criteria = new UserSearchCriteria();
			criteria.UserName.SortAsc(0);

			// create the criteria, depending on whether matches should be "exact" or "like"
			if(request.ExactMatchOnly)
			{
				if (!string.IsNullOrEmpty(request.UserName))
					criteria.UserName.EqualTo(request.UserName);
				if (!string.IsNullOrEmpty(request.DisplayName))
					criteria.DisplayName.EqualTo(request.DisplayName);
			}
			else
			{
				if (!string.IsNullOrEmpty(request.UserName))
					criteria.UserName.StartsWith(request.UserName);
				if (!string.IsNullOrEmpty(request.DisplayName))
					criteria.DisplayName.Like(string.Format("%{0}%", request.DisplayName));
			}

            UserAssembler assembler = new UserAssembler();
            List<UserSummary> userSummaries = CollectionUtils.Map<User, UserSummary>(
                PersistenceContext.GetBroker<IUserBroker>().Find(criteria, request.Page),
                delegate(User user)
                {
                    return assembler.GetUserSummary(user);
                });
            return new ListUsersResponse(userSummaries);
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request)
        {
            User user = FindUserByName(request.UserName);

            UserAssembler assembler = new UserAssembler();
            return new LoadUserForEditResponse(assembler.GetUserDetail(user));
        }

    	[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public AddUserResponse AddUser(AddUserRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserDetail, "UserDetail");

            UserDetail userDetail = request.UserDetail;

			// create new user
            User user = User.CreateNewUser(
                new UserInfo(userDetail.UserName, userDetail.DisplayName, userDetail.ValidFrom, userDetail.ValidUntil));

            // copy other info such as authority groups from request
            UserAssembler assembler = new UserAssembler();
            assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

			// save
            PersistenceContext.Lock(user, DirtyState.New);
    		PersistenceContext.SynchState();

            return new AddUserResponse(user.GetRef(), assembler.GetUserSummary(user));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public UpdateUserResponse UpdateUser(UpdateUserRequest request)
        {
            User user = FindUserByName(request.UserDetail.UserName);

            // update user account info
            UserAssembler assembler = new UserAssembler();
            assembler.UpdateUser(user, request.UserDetail, PersistenceContext);

            // reset password if requested
            if (request.UserDetail.ResetPassword)
                user.ResetPassword();

			PersistenceContext.SynchState();

			return new UpdateUserResponse(assembler.GetUserSummary(user));
        }

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public DeleteUserResponse DeleteUser(DeleteUserRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckForEmptyString(request.UserName, "UserName");

			// prevent current user from deleting own account
			if(request.UserName == Thread.CurrentPrincipal.Identity.Name)
				throw new RequestValidationException(SR.MessageCannotDeleteOwnUserAccount);

			IUserBroker broker = PersistenceContext.GetBroker<IUserBroker>();
			User user = FindUserByName(request.UserName);

			// remove user from groups we don't get errors from db references
			user.AuthorityGroups.Clear();

			// delete user
			broker.Delete(user);

			return new DeleteUserResponse();
		}

    	[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
		public ResetUserPasswordResponse ResetUserPassword(ResetUserPasswordRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserName, "UserName");

            User user = FindUserByName(request.UserName);
            user.ResetPassword();

            UserAssembler assembler = new UserAssembler();
            return new ResetUserPasswordResponse(assembler.GetUserSummary(user));
        }

    	#endregion

        private User FindUserByName(string name)
        {
            try
            {
                UserSearchCriteria where = new UserSearchCriteria();
                where.UserName.EqualTo(name);

                return PersistenceContext.GetBroker<IUserBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                throw new RequestValidationException(string.Format("{0} is not a valid user name.", name));
            }
        }
    }
}
