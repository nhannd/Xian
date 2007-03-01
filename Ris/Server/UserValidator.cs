using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace ClearCanvas.Ris.Server
{
    internal class UserValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            // TODO validate the user/password
            // (for now just allow open access)
            if (false)
            {
                throw new SecurityTokenException("Invalid user or password");
            }
        }
    }
}
