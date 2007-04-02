using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Server
{
    internal class CustomUserValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            Console.WriteLine("Validating user " + userName);
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                {
                    // TODO validate the password
                    if (!service.ValidateUser(userName))
                        throw new SecurityTokenException("Invalid user name or password");
                });
        }
    }
}
