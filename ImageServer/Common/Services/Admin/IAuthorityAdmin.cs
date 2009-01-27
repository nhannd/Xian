using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.ImageServer.Common.Services.Admin
{
    public interface IAuthorityAdminServices : IDisposable
    {
        LoginCredentials Credentials { set; }
        IList<AuthorityGroupSummary> ListAllAuthorityGroups();
        IList<AuthorityGroupSummary> ListAuthorityGroups(int pageIndex, int pageSize);
        IList<AuthorityTokenSummary> ListAuthorityTokens();
        AuthorityGroupSummary AddAuthorityGroup(string group, List<AuthorityTokenSummary> tokens);
        AuthorityGroupSummary UpdateAuthorityGroup(AuthorityGroupDetail detail);
        bool DeleteAuthorityGroup(AuthorityGroupSummary group);
        AuthorityGroupDetail LoadAuthorityGroupDetail(AuthorityGroupSummary group);
        bool ImportAuthorityGroups(List<AuthorityGroupDetail> groups);
        bool ImportAuthorityTokens(List<AuthorityTokenSummary> tokens);
    }
}
