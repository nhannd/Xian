using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Enterprise.Common
{
    public class AuthorityTokens
    {
        [AuthorityToken(Description = "Allow administration of application configuration")]
        public const string ConfigurationAdmin = "ConfigurationAdmin";
    }
}
