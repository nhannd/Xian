using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Server
{
    class CustomPrincipal : IPrincipal
    {
        private IIdentity _identity;

        public CustomPrincipal(IIdentity identity)
        {
            _identity = identity;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            bool b = false;
            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                {
                    b = service.AssertTokenForUser(_identity.Name, role);
                });
            return b;
        }

        #endregion
    }
}
