using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Authentication.Brokers
{
    public partial interface IAuthorityTokenBroker
    {
        /// <summary>
        /// Provides optimized retrieval of authority tokens for a given <see cref="User"/> according
        /// to user name.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        string[] FindTokensByUserName(string userName);

        /// <summary>
        /// Provides optimized assert that the specified user has the specified token.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        bool AssertUserHasToken(string userName, string token);
    }
}
