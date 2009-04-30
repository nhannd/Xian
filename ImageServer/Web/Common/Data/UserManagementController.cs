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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Enterprise.Admin;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class UserManagementController
	{
        public List<UserRowData> GetAllUsers()
        {
            List<UserRowData> data = null;
            
            using(UserManagement service = new UserManagement())
            {
                data = CollectionUtils.Map<UserSummary, UserRowData>(
                    service.FindUsers(new ListUsersRequest()),
                    delegate(UserSummary summary)
                    {
                        UserRowData user = new UserRowData(summary, null);
                        return user;
                    });
            }

            return data;
        }

        public bool AddUser(UserRowData user)
        {
            bool success = false;

            using(UserManagement service = new UserManagement())
            {
                try
                {
                    UserDetail newUser = new UserDetail();

                    newUser.UserName = user.UserName;
                    newUser.DisplayName = user.DisplayName;
                    newUser.Enabled = user.Enabled;
                    newUser.CreationTime = Platform.Time;
                    newUser.ResetPassword = true;

                    List<AuthorityGroupSummary> groups = new List<AuthorityGroupSummary>();

                    foreach (UserGroup userGroup in user.UserGroups)
                    {
                        groups.Add(new AuthorityGroupSummary(new EntityRef(userGroup.UserGroupRef), userGroup.Name));
                    }

                    newUser.AuthorityGroups = groups;

                    service.AddUser(newUser);
                    success = true;

                }
                catch (Exception ex)
                {
                	Platform.Log(LogLevel.Error, ex, "Unexpected exception adding user: {0}", user.DisplayName);
                }
            };

            return success;
        }

        public bool UpdateUser(UserRowData user)
        {
            bool success = false;

            using(UserManagement service = new UserManagement())
            {
                try
                {
                    UserDetail updateUser = new UserDetail();

                    updateUser.UserName = user.UserName;
                    updateUser.DisplayName = user.DisplayName;
                    updateUser.Enabled = user.Enabled;

                    List<AuthorityGroupSummary> groups = new List<AuthorityGroupSummary>();

                    foreach(UserGroup userGroup in user.UserGroups)
                    {
                        groups.Add(new AuthorityGroupSummary(new EntityRef(userGroup.UserGroupRef), userGroup.Name));
                    }

                    updateUser.AuthorityGroups = groups;

                    service.UpdateUserDetail(updateUser);
                    success = true;
                }
                catch (Exception ex)
                {
                	Platform.Log(LogLevel.Error, ex, "Unexpected exception updating user: {0}", user.DisplayName);
                }
            };

            return success;
        }

        public bool ExistsUsername(string username)
        {
            bool exists = false;

            using(UserManagement service = new UserManagement())
            {
                ListUsersRequest filter = new ListUsersRequest();
                filter.ExactMatchOnly = true;
                filter.UserName = username;

                IList<UserSummary> users = service.FindUsers(filter);

                if (users != null && users.Count > 0)
                {
                    exists = true;
                }
            };

            return exists;
        }

        public bool ResetPassword(UserRowData user)
        {
            bool success = false;

            using(UserManagement service = new UserManagement())
            {
                try
                {
                    service.ResetPassword(user.UserName);
                    success = true;
                }
                catch (Exception ex)
                {
                	Platform.Log(LogLevel.Error, ex, "Unexpected exception resetting password for user: {0}",
                	             user.DisplayName);
                }
            };

            return success;
        }

        public bool DeleteUser(UserRowData user)
        {
            bool success = false;

            using(UserManagement service = new UserManagement())
            {
                try
                {
                    service.DeleteUser(user.UserName);
                    success = true;
                }
                catch (Exception ex)
                {
					Platform.Log(LogLevel.Error, ex, "Unexpected exception deleting user: {0}",
								 user.DisplayName);
                }
            };

            return success;
        }

        public bool ExistsUsergroup(string usergroupName)
        {
            bool exists = false;

            using (AuthorityManagement service = new AuthorityManagement())
            {
                IList<AuthorityGroupSummary> list = service.ListAllAuthorityGroups();

            	if (usergroupName != null)
                {
                    foreach (AuthorityGroupSummary group in list)
                    {
                        if (group.Name.ToLower().Equals(usergroupName.ToLower()))
                        {
                            exists = true;
                            break;
                        }
                    }
                }
            };

            return exists;
        }

        public bool AddUserGroup(UserGroupRowData userGroup)
        {
            bool success = false;

            using(AuthorityManagement service = new AuthorityManagement())
            {
                List<AuthorityTokenSummary> tokens = new List<AuthorityTokenSummary>();

                foreach (TokenSummary token in userGroup.Tokens)
                {
                    tokens.Add(new AuthorityTokenSummary(token.Name, token.Description));
                }

                service.AddAuthorityGroup(userGroup.Name, tokens);
                success = true;
            };

            return success;
        }

        public bool UpdateUserGroup(UserGroupRowData userGroup)
        {
            bool success = false;

            using(AuthorityManagement service = new AuthorityManagement())
        
            {
                AuthorityGroupDetail detail = new AuthorityGroupDetail();
                detail.AuthorityGroupRef = new EntityRef(userGroup.Ref);
                detail.Name = userGroup.Name;
                
                foreach(TokenSummary token in userGroup.Tokens)
                {
                    detail.AuthorityTokens.Add(new AuthorityTokenSummary(token.Name, token.Description));
                }

                service.UpdateAuthorityGroup(detail);
                success = true;
            };

            return success;
        }

        public bool DeleteUserGroup(UserGroupRowData userGroup)
        {
            bool success = false;

            using(AuthorityManagement service = new AuthorityManagement())
            {
                try
                {
                    service.DeleteAuthorityGroup(new EntityRef(userGroup.Ref));
                    success = true;
                }
                catch (Exception ex)
                {
                	Platform.Log(LogLevel.Error, ex, "Unexpected exception deleting user group: {0}.", userGroup.Name);
                }
            };

            return success;
        }

        public bool UpdateTokens(List<TokenRowData> tokens)
        {
            bool success = false;

            using(AuthorityManagement service = new AuthorityManagement())
            {
                   List<AuthorityTokenSummary> tokenList = new List<AuthorityTokenSummary>();

                   foreach(TokenRowData token in tokens)
                   {
                       tokenList.Add(new AuthorityTokenSummary(token.Name, token.Description));
                   }

                   service.ImportAuthorityTokens(tokenList);
                   success = true;
            };

            return success;
        }
    }
}
