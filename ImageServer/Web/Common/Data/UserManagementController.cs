#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
