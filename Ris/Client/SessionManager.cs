using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Login;
using System.Threading;
using System.Security.Principal;

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
                        catch (Exception)
                        {
                            Platform.ShowMessageBox("Invalid user name or password");
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
