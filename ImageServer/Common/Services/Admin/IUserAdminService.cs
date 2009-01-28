using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.ImageServer.Common.Services.Admin
{
    public interface IUserAdminService:IDisposable
    {
        LoginCredentials Credentials { set; }
        List<UserSummary> ListUsers(ListUsersRequest filters);
        UserDetail GetUserDetail(string username);
        UserSummary UpdateUserDetail(UserDetail detail);
        bool DeleteUser(string username);
        UserSummary AddUser(UserDetail userdetail);
        bool ResetPassword(string username);
    }

}
