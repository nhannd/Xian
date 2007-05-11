using System;
using System.Collections.Generic;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Application.Services.Admin.AuthenticationAdmin
{
    internal class UserAssembler
    {
        internal UserSummary GetUserSummary(User user)
        {
            UserSummary summary = new UserSummary();
            summary.EntityRef = user.GetRef();
            summary.UserId = user.UserName;

            return summary;
        }

        internal UserDetail GetUserDetail(User user, Staff staff)
        {
            AuthorityGroupAssembler assembler = new AuthorityGroupAssembler();
            List<AuthorityGroupSummary> groups = new List<AuthorityGroupSummary>();
            foreach (AuthorityGroup authorityGroup in user.AuthorityGroups)
            {
                groups.Add(assembler.GetAuthorityGroupSummary(authorityGroup));
            }

            UserDetail userDetail = new UserDetail();
            userDetail.UserId = user.UserName;
            userDetail.AuthorityGroups = groups;

            if(staff != null)
            {
                PersonNameAssembler pnAssembler = new PersonNameAssembler();
                userDetail.Name = pnAssembler.CreatePersonNameDetail(staff.Name);
                userDetail.StaffRef = staff.GetRef();
            }

            return userDetail;
        }

        internal void UpdateUser(User user, UserDetail detail, IPersistenceContext context)
        {
            user.UserName = detail.UserId;

            user.AuthorityGroups.Clear();
            foreach (AuthorityGroupSummary group in detail.AuthorityGroups)
            {
                user.AuthorityGroups.Add((AuthorityGroup)context.Load(group.EntityRef));
            }
        }
    }
}
