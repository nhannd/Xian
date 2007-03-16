using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Authentication
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    public class AuthenticationService : CoreServiceLayer, IAuthenticationService
    {
        #region IAuthenticationService Members

        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public bool ValidateUser(string userName)
        {
            // TODO expand this to include the concept of a password

            UserSearchCriteria where = new UserSearchCriteria();
            where.UserName.EqualTo(userName);

            long count = CurrentContext.GetBroker<IUserBroker>().Count(where);
            return count == 1;
        }

        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public string[] ListPermissionsForUser(string userName)
        {
            return CurrentContext.GetBroker<IPermissionBroker>().FindPermissionsByUserName(userName);
        }

        #endregion
    }
}
