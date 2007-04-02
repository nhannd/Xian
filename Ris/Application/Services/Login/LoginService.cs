using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Login;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services.Login
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ILoginService))]
    public class LoginService : ApplicationServiceBase, ILoginService
    {
        #region ILoginService Members

        [ReadOperation]
        public LoginResponse Login(LoginRequest request)
        {
            string[] authorityTokens = null;

            Platform.GetService<IAuthenticationService>(
                delegate(IAuthenticationService service)
                {
                    authorityTokens = service.ListAuthorityTokensForUser(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                });

            return new LoginResponse(authorityTokens);
        }

        #endregion
    }
}
