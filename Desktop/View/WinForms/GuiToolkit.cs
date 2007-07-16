using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.ComponentModel;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(GuiToolkitExtensionPoint))]
    public class GuiToolkit : IGuiToolkit
    {
        public GuiToolkit()
        {
            if (!Platform.IsWin32Platform)
                throw new NotSupportedException();
        }

        #region IGuiToolkit Members

        public void Initialize()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
        }

        public string ToolkitID
        {
            get { return GuiToolkitID.WinForms; }
        }

        public void RunMessagePump()
        {
            System.Windows.Forms.Application.Run();
        }

        public void QuitMessagePump()
        {
            System.Windows.Forms.Application.Exit();
        }

        #endregion
    }
}
