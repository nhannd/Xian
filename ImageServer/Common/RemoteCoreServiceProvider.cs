using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = true)]
    class RemoteServiceProvider : RemoteCoreServiceProvider
    {
        protected override string UserName
        {
            get
            {
                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                Platform.CheckForNullReference(principal, "principal");
                Platform.CheckMemberIsSet(principal.Credentials, "principal.Credentials");
                return principal.Credentials.UserName;
            }
        }

        protected override string Password
        {
            get
            {
                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                Platform.CheckForNullReference(principal, "principal");
                Platform.CheckMemberIsSet(principal.Credentials, "principal.Credentials");
                Platform.CheckMemberIsSet(principal.Credentials.SessionToken, "principal.Credentials.SessionToken");
                return principal.Credentials.SessionToken.Id;
            }
        }
    }
}