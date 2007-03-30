using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Provides methods for validating user credentials and obtaining a list of permissions that a user has
    /// been granted.
    /// </summary>
    /// <remarks>
    /// This service cannot itself be protected, because it is used to provide protection to other services.
    /// </remarks>
    [ServiceContract]
    public interface IAuthenticationService
    {
        [OperationContract]
        bool ValidateUser(string userName);

        [OperationContract]
        string[] ListAuthorityTokensForUser(string userName);
    }
}
