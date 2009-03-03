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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Services.Admin;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using IUserAdminService = ClearCanvas.ImageServer.Common.Services.Admin.IUserAdminService;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class UserManagementController
	{
        public List<UserRowData> GetAllUsers()
        {
            List<UserRowData> data = new List<UserRowData>();
            
            Platform.GetService<IUserAdminService>(
                    delegate(IUserAdminService services)
                    {
                        List<UserSummary> users = services.ListUsers(new ListUsersRequest());

                        List<UserRowData> rows = CollectionUtils.Map<UserSummary, UserRowData>(
                            users, delegate(UserSummary summary)
                                       {
                                           UserRowData row = new UserRowData(summary, services.GetUserDetail(summary.UserName));
                                           return row;
                                       });

                        data = rows;
                    });

            return data;
        }

        public bool AddUser(UserRowData user)
        {
            bool success = false;

            Platform.GetService<IUserAdminService>(
                delegate(IUserAdminService service)
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
                        throw ex;
                    }
                });

            return success;
        }

        public bool UpdateUser(UserRowData user)
        {
            bool success = false;

            Platform.GetService<IUserAdminService>(
                delegate(IUserAdminService service)
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
                        //TODO: Handle Exception
                    }
                });

            return success;
        }

        public bool ExistsUsername(string username)
        {
            bool exists = false;

            Platform.GetService<IUserAdminService>(
        delegate(IUserAdminService services)
        {
            ListUsersRequest filter = new ListUsersRequest();
            filter.ExactMatchOnly = true;
            filter.UserName = username;

            List<UserSummary> users = services.ListUsers(filter);

            if (users.Count > 0)
            {
                exists = true;
            }
        });

            return exists;
        }

        public bool ResetPassword(UserRowData user)
        {
            bool success = false;

            Platform.GetService<IUserAdminService>(
                delegate(IUserAdminService service)
                {
                    try
                    {
                        service.ResetPassword(user.UserName);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        //TODO: Handle Exception
                    }
                });

            return success;
        }

        public bool DeleteUser(UserRowData user)
        {
            bool success = false;

            Platform.GetService<IUserAdminService>(
                delegate(IUserAdminService service)
                {
                    try
                    {
                        service.DeleteUser(user.UserName);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        //TODO: Handle Exception
                    }
                });

            return success;
        }

        public bool ExistsUsergroup(string usergroupName)
        {
            bool exists = false;

            Platform.GetService<IAuthorityAdminService>(
                            delegate(IAuthorityAdminService services)
                            {
                                IList<AuthorityGroupSummary> list = services.ListAllAuthorityGroups();
                                IList<AuthorityGroupSummary> filteredList = new List<AuthorityGroupSummary>();

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
                            });

            return exists;
        }

        public bool AddUserGroup(UserGroupRowData userGroup)
        {
            bool success = false;

            Platform.GetService<IAuthorityAdminService>(
                delegate(IAuthorityAdminService service)
                {
                    List<AuthorityTokenSummary> tokens = new List<AuthorityTokenSummary>();

                    foreach (TokenSummary token in userGroup.Tokens)
                    {
                        tokens.Add(new AuthorityTokenSummary(token.Name, token.Description));
                    }

                    service.AddAuthorityGroup(userGroup.Name, tokens);
                    success = true;
                });

            return success;
        }

        public bool UpdateUserGroup(UserGroupRowData userGroup)
        {
            bool success = false;

            Platform.GetService<IAuthorityAdminService>(
                delegate(IAuthorityAdminService service)
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
                });

            return success;
        }

        public bool DeleteUserGroup(UserGroupRowData userGroup)
        {
            bool success = false;

            Platform.GetService<IAuthorityAdminService>(
                delegate(IAuthorityAdminService service)
                {
                    try
                    {
                        service.DeleteAuthorityGroup(new AuthorityGroupSummary(new EntityRef(userGroup.Ref), userGroup.Name));
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        //TODO: Handle Exception
                    }
                });

            return success;
        }

        public bool UpdateTokens(List<TokenRowData> tokens)
        {
            bool success = false;

            Platform.GetService<IAuthorityAdminService>(
               delegate(IAuthorityAdminService service)
               {
                   List<AuthorityTokenSummary> tokenList = new List<AuthorityTokenSummary>();

                   foreach(TokenRowData token in tokens)
                   {
                       tokenList.Add(new AuthorityTokenSummary(token.Name, token.Description));
                   }

                   service.ImportAuthorityTokens(tokenList);
                   success = true;
               });

            return success;
        }
    }
}
