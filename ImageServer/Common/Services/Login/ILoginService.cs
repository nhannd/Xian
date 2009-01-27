using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.ImageServer.Common.Services.Login
{
    public interface ILoginService:IDisposable
    {
        /// <summary>
        /// Logs into the system using specified username and password.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        SessionInfo Login(string userName, string password);

        /// <summary>
        /// Logs out of the system.
        /// </summary>
        /// <param name="session"></param>
        void Logout(SessionInfo session);

        /// <summary>
        /// Change user's password.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        void ChangePassword(string userName, string oldPassword, string newPassword);

        /// <summary>
        /// Validate the specified session. Refresh the session data if needed.
        /// </summary>
        /// <param name="session"></param>
        void Validate(SessionInfo session);
    }
}
