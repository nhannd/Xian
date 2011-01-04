#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;

namespace ClearCanvas.Enterprise.Authentication.Admin.UserAdmin
{
    internal class UserAssembler
    {
        internal UserSummary GetUserSummary(User user)
        {
            return new UserSummary(user.UserName, user.DisplayName, user.CreationTime, user.ValidFrom, user.ValidUntil,
                user.LastLoginTime, user.Enabled);
        }

        internal UserDetail GetUserDetail(User user)
        {
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();

        	List<AuthorityGroupSummary> groups = CollectionUtils.Map<AuthorityGroup, AuthorityGroupSummary>(
        		user.AuthorityGroups,
        		delegate(AuthorityGroup group)
        		{
        			return assembler.CreateAuthorityGroupSummary(group);
        		});
				
            return new UserDetail(user.UserName, user.DisplayName, user.CreationTime, user.ValidFrom, user.ValidUntil,
                user.LastLoginTime, user.Enabled, user.Password.ExpiryTime, groups);
        }

        internal void UpdateUser(User user, UserDetail detail, IPersistenceContext context)
        {
            // do not update user.UserName
            // do not update user.Password
            user.DisplayName = detail.DisplayName;
            user.ValidFrom = detail.ValidFrom;
            user.ValidUntil = detail.ValidUntil;
            user.Enabled = detail.Enabled;
            user.Password.ExpiryTime = detail.PasswordExpiryTime;

            // process authority groups
			List<AuthorityGroup> authGroups = CollectionUtils.Map<AuthorityGroupSummary, AuthorityGroup>(
				detail.AuthorityGroups,
                delegate(AuthorityGroupSummary group)
                {
                	return context.Load<AuthorityGroup>(group.AuthorityGroupRef, EntityLoadFlags.Proxy);
                });

            user.AuthorityGroups.Clear();
			user.AuthorityGroups.AddAll(authGroups);
        }
    }
}
