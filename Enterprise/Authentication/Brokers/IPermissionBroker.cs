using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Authentication.Brokers
{
    public partial interface IPermissionBroker
    {
        /// <summary>
        /// Provides optimized retrieval of permissions for a given <see cref="User"/> according
        /// to user name.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        string[] FindPermissionsByUserName(string userName);
    }
}
