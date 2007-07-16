using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(ApplicationViewExtensionPoint))]
    public class ApplicationView : WinFormsView, IApplicationView
    {
        public ApplicationView()
        {
        }

        #region IApplicationView Members

        public IDesktopWindowView OpenWindow(DesktopWindow window)
        {
            return new DesktopWindowView(window);
        }

        public DialogBoxAction ShowMessageBox(string message, MessageBoxActions actions)
        {
            MessageBox mb = new MessageBox();
            return mb.Show(message, actions);
        }

        #endregion

        public override object GuiElement
        {
            // not used
            get { return null; }
        }
    }
}
