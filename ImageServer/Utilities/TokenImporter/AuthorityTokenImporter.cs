using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Admin;
using ClearCanvas.ImageServer.Common.Services.Login;

namespace ClearCanvas.ImageServer.Utilities
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class TokenImporter : IApplicationRoot
    {
        SessionInfo _session;

        #region IApplicationRoot Members
        
        
        public void RunApplication(string[] args)
        {
            CommandLine cmd = new CommandLine(args);
            string user = cmd.Named["u"];
            string pwd = cmd.Named["p"];

            Platform.GetService<ILoginService>(delegate(ILoginService service)
                                                   {
                                                       try
                                                       {
                                                           _session = service.Login(user, pwd);
                                                       }
                                                       catch(PasswordExpiredException ex)
                                                       {
                                                           Console.WriteLine("Password expired. Reseting password..");
                                                           service.ChangePassword(user, pwd, "clearcanvas123");
                                                           service.ChangePassword(user, "clearcanvas123", pwd);
                                                           Console.WriteLine("Attempt to login again...");
                                                           _session = service.Login(user, pwd);
                                                       }
                                                   });
            AuthorityTokenDefinition[] tokenDefs = AuthorityGroupSetup.GetAuthorityTokens();
            Import(tokenDefs);
        }

        public IList<AuthorityToken> Import(IEnumerable<AuthorityTokenDefinition> tokenDefs)
        {
            List<AuthorityTokenSummary> tokens = CollectionUtils.Map<AuthorityTokenDefinition, AuthorityTokenSummary>(
                tokenDefs,
                delegate(AuthorityTokenDefinition def)
                    {
                        AuthorityTokenSummary token = new AuthorityTokenSummary(def.Token, def.Description);
                        return token;
                    }
                );
            Platform.GetService<IAuthorityAdminService>(
                delegate(IAuthorityAdminService service)
                    {
                        service.Credentials = _session.Credentials;
                        service.ImportAuthorityTokens(tokens);
                        
                    });
            

            return null;
        }

        #endregion
    }
}
