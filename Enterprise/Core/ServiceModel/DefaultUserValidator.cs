using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Authentication;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
    /// <summary>
    /// Implemenation of <see cref="UserNamePasswordValidator"/> that authenticates
    /// a user via the <see cref="IAuthenticationService"/>.
    /// </summary>
    class DefaultUserValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            Platform.Log(LogLevel.Debug, "Validating session for user ", userName);

			// Note: password is actually the session token

			Platform.GetService<IAuthenticationService>(
				delegate(IAuthenticationService service)
					{
						// this call will throw an exception if the session is invalid or has expired
						service.ValidateSession(new ValidateSessionRequest(userName, new SessionToken(password)));
					});
		}
	}
}