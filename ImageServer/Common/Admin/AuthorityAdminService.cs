using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.ImageServer.Common.Admin
{
    public sealed class AuthorityAdminService : IDisposable
    {
        private IAuthorityGroupAdminService _service;

        public AuthorityAdminService()
        {
            _service =  Platform.GetService<IAuthorityGroupAdminService>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_service != null && _service is IDisposable)
            {
                (_service as IDisposable).Dispose();
                _service = null;
            }
        }

        #endregion



        public IList<AuthorityGroupSummary> ListAllAuthorityGroups()
        {
            return _service.ListAuthorityGroups(new ListAuthorityGroupsRequest()).AuthorityGroups;
        }

        public void AddAuthorityGroup(string name, List<AuthorityTokenSummary> tokens)
        {
            AuthorityGroupDetail details = new AuthorityGroupDetail();
            details.Name = name;
            details.AuthorityTokens = tokens;
            _service.AddAuthorityGroup(new AddAuthorityGroupRequest(details));
        }

        public void UpdateAuthorityGroup(AuthorityGroupDetail detail)
        {
            _service.UpdateAuthorityGroup(new UpdateAuthorityGroupRequest(detail));
        }

        public void DeleteAuthorityGroup(EntityRef entityRef)
        {
            _service.DeleteAuthorityGroup(new DeleteAuthorityGroupRequest(entityRef));
        }

        public void ImportAuthorityTokens(List<AuthorityTokenSummary> tokens)
        {
            _service.ImportAuthorityTokens(new ImportAuthorityTokensRequest(tokens));
        }

        public AuthorityGroupDetail LoadAuthorityGroupDetail(AuthorityGroupSummary group)
        {
            return
                _service.LoadAuthorityGroupForEdit(new LoadAuthorityGroupForEditRequest(group.AuthorityGroupRef)).
                    AuthorityGroupDetail;
        }

        public IList<AuthorityTokenSummary> ListAuthorityTokens()
        {
            return _service.ListAuthorityTokens(new ListAuthorityTokensRequest()).AuthorityTokens;
        }

        public bool ImportAuthorityGroups(List<AuthorityGroupDetail> groups)
        {
            ImportAuthorityGroupsRequest request = new ImportAuthorityGroupsRequest(groups);
            return _service.ImportAuthorityGroups(request)!=null;
        }
    }
}