using System;
using System.Collections.Generic;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.AuthenticationAdmin
{
    internal class UserAssembler
    {
        internal UserSummary GetUserSummary(User user)
        {
            return new UserSummary(
                user.GetRef(),
                user.UserName,
                new PersonNameDetail()
                );
        }

        internal UserDetail GetUserDetail(User user)
        {
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            List<AuthorityGroupSummary> groups = new List<AuthorityGroupSummary>();
            foreach (AuthorityGroup authorityGroup in user.AuthorityGroups)
            {
                groups.Add(assembler.GetAuthorityGroupSummary(authorityGroup));
            }

            return new UserDetail(
                user.UserName,
                new PersonNameDetail(),
                groups
                );
        }

        internal void UpdateUser(User user, UserDetail detail, IPersistenceContext context)
        {
        }
    }
}
