using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Login;
using System.Threading;
using System.Security.Principal;
using System.ServiceModel;

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
                            Platform.ShowMessageBox(SR.MessageCommunicationError);
                        }
                        catch (TimeoutException)
                        {
                            Platform.ShowMessageBox(SR.MessageLoginTimeout);
                        }
                        catch (Exception)
                        {
                            Platform.ShowMessageBox(SR.MessageInvalidUserNamePassword);
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
