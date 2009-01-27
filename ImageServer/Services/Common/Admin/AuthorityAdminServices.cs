using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Admin;

namespace ClearCanvas.ImageServer.Services.Common.Admin
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    public class AuthorityAdminServices : ICoreServiceLayer, IAuthorityAdminServices, IDisposable
    {
        private AuthorityAdminServiceClient _client = new AuthorityAdminServiceClient();
        private LoginCredentials _credentials;
        private bool _clientCredentialsSet = false;
        public LoginCredentials Credentials
        {
            set { _credentials = value; }
            private get
            {
                if (_credentials == null)
                {
                    return LoginCredentials.Current;
                }
                else
                {
                    return _credentials;
                }
            }
        }
        #region IDisposable Members

        public void Dispose()
        {
            if (_client != null)
            {
                if (_client.State != CommunicationState.Closed)
                {
                    _client.Close();
                }
            }
        }


        #endregion

        public IList<AuthorityGroupSummary> ListAuthorityGroups(int pageIndex, int pageSize)
        {
            SetupCredentials();
            ListAuthorityGroupsRequest request = new ListAuthorityGroupsRequest();
            ListAuthorityGroupsResponse response = _client.ListAuthorityGroups(request);
            
            return response.AuthorityGroups;
        }

        public IList<AuthorityGroupSummary> ListAllAuthorityGroups()
        {
            SetupCredentials();
            ListAuthorityGroupsRequest request = new ListAuthorityGroupsRequest();
            ListAuthorityGroupsResponse response = _client.ListAuthorityGroups(request);

            return response.AuthorityGroups;
        }

        public AuthorityGroupSummary AddAuthorityGroup(string group, List<AuthorityTokenSummary> tokens)
        {
            SetupCredentials();
            AuthorityGroupDetail detail = new AuthorityGroupDetail();
            detail.Name = group;
            detail.AuthorityTokens = tokens;
            AddAuthorityGroupRequest request = new AddAuthorityGroupRequest(detail);
            AddAuthorityGroupResponse response = _client.AddAuthorityGroup(request);
            return response.AuthorityGroupSummary;
        }

        private void SetupCredentials()
        {
            if (!_clientCredentialsSet)
            {
                _client.Credentials = this.Credentials;
                _clientCredentialsSet = true;
            }
        }

        public AuthorityGroupSummary UpdateAuthorityGroup(AuthorityGroupDetail detail)
        {
            SetupCredentials();
            UpdateAuthorityGroupRequest request = new UpdateAuthorityGroupRequest(detail);
            UpdateAuthorityGroupResponse response = _client.UpdateAuthorityGroup(request);
            return response.AuthorityGroupSummary;
        }

        public bool DeleteAuthorityGroup(AuthorityGroupSummary group)
        {
            SetupCredentials();
            DeleteAuthorityGroupRequest request = new DeleteAuthorityGroupRequest(group.AuthorityGroupRef);
            DeleteAuthorityGroupResponse response = _client.DeleteAuthorityGroup(request);
            return response!=null;
        }

        public AuthorityGroupDetail LoadAuthorityGroupDetail(AuthorityGroupSummary group)
        {
            SetupCredentials();
            LoadAuthorityGroupForEditRequest request = new LoadAuthorityGroupForEditRequest(group.AuthorityGroupRef);
            LoadAuthorityGroupForEditResponse response = _client.LoadAuthorityGroupForEdit(request);
            return response.AuthorityGroupDetail;
        }

        public IList<AuthorityTokenSummary> ListAuthorityTokens()
        {
            SetupCredentials();
            ListAuthorityTokensRequest request = new ListAuthorityTokensRequest();
            ListAuthorityTokensResponse response = _client.ListAuthorityTokens(request);
            return response.AuthorityTokens;
        }

        public bool ImportAuthorityGroups(List<AuthorityGroupDetail> groups)
        {
            SetupCredentials();
            ImportAuthorityGroupsRequest request = new ImportAuthorityGroupsRequest(groups);
            ImportAuthorityGroupsResponse response = _client.ImportAuthorityGroups(request);
            return response != null;
        }

        public bool ImportAuthorityTokens(List<AuthorityTokenSummary> tokens)
        {
            SetupCredentials();
            ImportAuthorityTokensRequest request = new ImportAuthorityTokensRequest(tokens);
            ImportAuthorityTokensResponse response = _client.ImportAuthorityTokens(request);
            return response!=null;
        }
    }

    class AuthorityAdminServiceClient : ClientBase<IAuthorityGroupAdminService>, IAuthorityGroupAdminService
    {
        public LoginCredentials Credentials
        {
            set
            {
                base.ClientCredentials.UserName.UserName = value.UserName;
                base.ClientCredentials.UserName.Password = value.SessionToken.Id;
            }
        }

        #region IAuthorityGroupAdminService Members

        public ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request)
        {
            return base.Channel.ListAuthorityGroups(request);
        }

        public AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request)
        {
            return base.Channel.AddAuthorityGroup(request);
        }

        public UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request)
        {
            return base.Channel.UpdateAuthorityGroup(request);
        }

        public DeleteAuthorityGroupResponse DeleteAuthorityGroup(DeleteAuthorityGroupRequest request)
        {
            return base.Channel.DeleteAuthorityGroup(request);
        }

        public LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request)
        {
            return base.Channel.LoadAuthorityGroupForEdit(request);
        }

        public ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request)
        {
            return base.Channel.ListAuthorityTokens(request);
        }

        public ImportAuthorityTokensResponse ImportAuthorityTokens(ImportAuthorityTokensRequest request)
        {
            return base.Channel.ImportAuthorityTokens(request);
        }

        public ImportAuthorityGroupsResponse ImportAuthorityGroups(ImportAuthorityGroupsRequest request)
        {
            return base.Channel.ImportAuthorityGroups(request);
        }

        #endregion
    }
}
