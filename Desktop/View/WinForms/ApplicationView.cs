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
    /// <para>
    /// This class may subclassed if customization is desired.
    /// Reasons for subclassing may include: overriding the <see cref="CreateDesktopWindowView"/>
    /// factory method to supply a custom subclasses of <see cref="DesktopWindowView"/>,
    /// and overriding <see cref="ShowMessageBox"/> to customize the display of message boxes.
    /// </para>
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
        /// <remarks>
        /// Override this method if you want to return a custom implementation of <see cref="IDesktopWindowView"/>.
        /// In practice, it is preferable to subclass <see cref="DesktopWindowView"/> rather than implement <see cref="IDesktopWindowView"/>
        /// directly.
        /// </remarks>
        /// <param name="window"></param>
        /// <returns></returns>
        public virtual IDesktopWindowView CreateDesktopWindowView(DesktopWindow window)
        {
            return new DesktopWindowView(window);
        }

        /// <summary>
        /// Displays a message box.
        /// </summary>
        /// <remarks>
        /// Override this method if you need to customize the display of message boxes.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public virtual DialogBoxAction ShowMessageBox(string message, MessageBoxActions actions)
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
