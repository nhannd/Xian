using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Provides methods for validating user credentials and obtaining a list of permissions that a user has
    /// been granted.
    /// </summary>
    /// <remarks>
    /// This service cannot itself be protected, because it is used to provide protection to other services.
    /// </remarks>
    public interface IAuthenticationService
    {
        bool ValidateUser(string userName);
        string[] ListPermissionsForUser(string userName);
    }
}
