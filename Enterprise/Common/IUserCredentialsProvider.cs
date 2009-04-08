using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Defines an interface that supplies user credentials.
    /// </summary>
    public interface IUserCredentialsProvider
    {
        /// <summary>
        /// Gets the user-name.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Gets the session token ID.
        /// </summary>
        string SessionTokenId { get; }
    }
}
