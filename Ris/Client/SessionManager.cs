using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Login;
using System.Threading;
using System.Security.Principal;
using System.ServiceModel;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class LoginDialogExtensionPoint : ExtensionPoint<ILoginDialog>
    {
    }


    [ExtensionOf(typeof(SessionManagerExtensionPoint))]
    class SessionManager : ISessionManager
    {
        #region ISessionManager Members

        public bool InitiateSession()
        {
            using (ILoginDialog loginDialog = (ILoginDialog)(new LoginDialogExtensionPoint()).CreateExtension())
            {
                while (true)
                {
                    string userName;
                    string password;

                    if (loginDialog.Show(out userName, out password))
                    {
                        try
                        {
                            LoginSession.Create(userName, password);

                            // successfully logged in
                            return true;
                        }
                        catch (CommunicationException)
                        {
                            ClearCanvas.Desktop.Application.ShowMessageBox(SR.MessageCommunicationError, MessageBoxActions.Ok);
                        }
                        catch (TimeoutException)
                        {
                            ClearCanvas.Desktop.Application.ShowMessageBox(SR.MessageLoginTimeout, MessageBoxActions.Ok);
                        }
                        catch (Exception)
                        {
                            ClearCanvas.Desktop.Application.ShowMessageBox(SR.MessageInvalidUserNamePassword, MessageBoxActions.Ok);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public void TerminateSession()
        {
        }

        #endregion
    }
}
