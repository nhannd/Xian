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
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.Web.Enterprise.Admin
{
    /// <summary>
    /// Wrapper for <see cref="IUserAdminService"/> service.
    /// </summary>
    /// 
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public sealed class UserManagement : IDisposable
    {
        private IUserAdminService _service;

        public UserManagement()
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