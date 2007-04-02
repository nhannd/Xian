using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    [ExtensionOf(typeof(LoginDialogExtensionPoint))]
    public class LoginDialog : ILoginDialog
    {
        #region ILoginDialog Members

        public bool Show(out string userName, out string password)
        {
            LoginForm form = new LoginForm();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                userName = form.UserName;
                password = null;
                return true;
            }
            else
            {
                userName = null;
                password = null;
                return false;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // nothing to do
        }

        #endregion
    }
}
