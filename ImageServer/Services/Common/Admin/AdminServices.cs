using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Admin;


namespace ClearCanvas.ImageServer.Services.Common.Admin
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    public class AdminServices : ICoreServiceLayer, IAdminServices, IDisposable
    {
        private AdminServiceClient _client = new AdminServiceClient();
        private LoginCredentials _credentials;
        private bool _clientCredentialsSet;

        public LoginCredentials Credentials
        {
            set { _credentials = value; }
            private get
            {
                if (_credentials==null)
                {
                    return LoginCredentials.Current;  
                }
                else
                {
                    return _credentials;
                }
            }
        }
        private void SetupCredentials()
        {
            if (!_clientCredentialsSet)
            {
                _client.Credentials = this.Credentials;
                _clientCredentialsSet = true;
            }
        }

        #region IAdminServices Members

        public List<UserSummary> ListUsers(ListUsersRequest filters)
        {
            SetupCredentials();
            ListUsersResponse response = _client.ListUsers(filters);
            return response.Users;
        }


        #endregion

        #region IAdminServices Members


        public UserDetail GetUserDetail(string username)
        {
            SetupCredentials();
            LoadUserForEditRequest request = new LoadUserForEditRequest(username);
            LoadUserForEditResponse response = _client.LoadUserForEdit(request);
            return response.UserDetail;
        }

        public UserSummary UpdateUserDetail(UserDetail detail)
        {
            SetupCredentials();
            UpdateUserRequest request = new UpdateUserRequest(detail);
            UpdateUserResponse response = _client.UpdateUser(request);
            return response.UserSummary;
        }

        public bool DeleteUser(string username)
        {

            SetupCredentials();
            DeleteUserRequest request = new DeleteUserRequest(username);
            DeleteUserResponse response = _client.DeleteUser(request);
        
            return true;
        }

        public UserSummary AddUser(UserDetail detail)
        {
            SetupCredentials();
            AddUserRequest request = new AddUserRequest(detail);
            AddUserResponse response = _client.AddUser(request);
            return response.UserSummary;
            
        }

        #endregion

        #region IAdminServices Members


        public bool ResetPassword(string username)
        {
            SetupCredentials();
            ResetUserPasswordRequest request = new ResetUserPasswordRequest(username);
            ResetUserPasswordResponse response = _client.ResetUserPassword(request);
            return true;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_client!=null)
            {
                if (_client.State != CommunicationState.Closed)
                {
                    _client.Close();
                }
            }
        }

        #endregion
    }


    class AdminServiceClient : ClientBase<IUserAdminService>, IUserAdminService
    {
        public LoginCredentials Credentials {
            set
            {
                base.ClientCredentials.UserName.UserName = value.UserName;
                base.ClientCredentials.UserName.Password = value.SessionToken.Id;
            }
        }

        #region IUserAdminService Members

        public ListUsersResponse ListUsers(ListUsersRequest request)
        {
            return base.Channel.ListUsers(request);
        }

        public AddUserResponse AddUser(AddUserRequest request)
        {
            return base.Channel.AddUser(request);
        }

        public UpdateUserResponse UpdateUser(UpdateUserRequest request)
        {
            return base.Channel.UpdateUser(request);
        }

        public DeleteUserResponse DeleteUser(DeleteUserRequest request)
        {
            return base.Channel.DeleteUser(request);
        }

        public ResetUserPasswordResponse ResetUserPassword(ResetUserPasswordRequest request)
        {
            return base.Channel.ResetUserPassword(request);
        }

        public LoadUserForEditResponse LoadUserForEdit(LoadUserForEditRequest request)
        {
            return base.Channel.LoadUserForEdit(request);
        }

        #endregion
    }
}
