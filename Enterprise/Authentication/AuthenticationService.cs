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
    public class AuthenticationService : CoreServiceLayer, IAuthenticationService
    {
        #region IAuthenticationService Members

        [ReadOperation]
        public bool ValidateUser(string userName)
        {
            // TODO expand this to include the concept of a password

            UserSearchCriteria where = new UserSearchCriteria();
            where.UserName.EqualTo(userName);

            int count = PersistenceContext.GetBroker<IUserBroker>().Count(where);
            return count == 1;
        }

        [ReadOperation]
        public string[] ListAuthorityTokensForUser(string userName)
        {
            return PersistenceContext.GetBroker<IAuthorityTokenBroker>().FindTokensByUserName(userName);
        }

        [ReadOperation]
        public bool AssertTokenForUser(string userName, string token)
        {
            return PersistenceContext.GetBroker<IAuthorityTokenBroker>().AssertUserHasToken(userName, token);
        }


        #endregion
    }
}
