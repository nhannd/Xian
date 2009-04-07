using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.ImageServer.Common.Admin
{
    /// <summary>
    /// Provides services
    /// </summary>
    public sealed class UserAdminService: IDisposable
    {
        private IUserAdminService _service;

        public UserAdminService()
        {
            _service = Platform.GetService<IUserAdminService>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_service!=null && _service is IDisposable)
            {
                (_service as IDisposable).Dispose();
                _service = null;
            }
        }

        #endregion

        public UserDetail GetUserDetail(string name)
        {
            return _service.LoadUserForEdit(new LoadUserForEditRequest(name)).UserDetail;
        }


        public void AddUser(UserDetail user)
        {
            _service.AddUser(new AddUserRequest(user));
        }

        public void UpdateUserDetail(UserDetail user)
        {
            _service.UpdateUser(new UpdateUserRequest(user));
        }

        public void ResetPassword(string name)
        {
            _service.ResetUserPassword(new ResetUserPasswordRequest(name));
        }

        public void DeleteUser(string name)
        {
            _service.DeleteUser(new DeleteUserRequest(name));
        }

        public IList<UserSummary> FindUsers(ListUsersRequest filter)
        {
            List<UserSummary> users = _service.ListUsers(filter).Users;

            return users;
        }
    }
}