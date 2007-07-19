using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IApplicationView"/>. 
    /// </summary>
    /// <remarks>
    /// This class acts as a view onto the application as a whole. It may subclassed if customization is desired.
    /// </remarks>
    [ExtensionOf(typeof(ApplicationViewExtensionPoint))]
    public class ApplicationView : WinFormsView, IApplicationView
    {
        /// <summary>
        /// No-args constructor required by extension point framework.
        /// </summary>
        public ApplicationView()
        {
        }

        #region IApplicationView Members

        /// <summary>
        /// Creates a new view for the specified <see cref="DesktopWindow"/>.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public IDesktopWindowView CreateDesktopWindowView(DesktopWindow window)
        {
            return new DesktopWindowView(window);
        }

        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public DialogBoxAction ShowMessageBox(string message, MessageBoxActions actions)
        {
            MessageBox mb = new MessageBox();
            return mb.Show(message, actions);
        }

        #endregion

        /// <summary>
        /// Not used by this class.
        /// </summary>
        public override object GuiElement
        {
            // not used
            get { throw new NotSupportedException(); }
        }
    }
}
