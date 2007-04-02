using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Authentication
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IAuthenticationService))]
    [ServiceAuthentication(false)]
    public class AuthenticationService : CoreServiceLayer, IAuthenticationService
    {
        #region IAuthenticationService Members

        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public bool ValidateUser(string userName)
        {
            // TODO expand this to include the concept of a password

            UserSearchCriteria where = new UserSearchCriteria();
            where.UserName.EqualTo(userName);

            int count = PersistenceContext.GetBroker<IUserBroker>().Count(where);
            return count == 1;
        }

        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public string[] ListAuthorityTokensForUser(string userName)
        {
            return PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(userName);
        }

        #endregion
    }
}
