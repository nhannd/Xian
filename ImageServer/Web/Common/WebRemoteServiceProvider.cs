using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Common
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = true)]
    class WebRemoteServiceProvider : RemoteCoreServiceProvider
    {
        protected override string UserName
        {
            get
            {
                Platform.CheckForNullReference(SessionManager.Current, " SessionManager.Current");
                Platform.CheckForNullReference(SessionManager.Current.Credentials, "SessionManager.Current.Credentials");
                return SessionManager.Current.Credentials.UserName;
            }
        }

        protected override string Password
        {
            get
            {
                Platform.CheckForNullReference(SessionManager.Current, " SessionManager.Current");
                Platform.CheckForNullReference(SessionManager.Current.Credentials, "SessionManager.Current.Credentials");
                Platform.CheckForNullReference(SessionManager.Current.Credentials.SessionToken, "SessionManager.Current.Credentials.SessionToken");
                return SessionManager.Current.Credentials.SessionToken.Id;
            }
        }
    }
}
