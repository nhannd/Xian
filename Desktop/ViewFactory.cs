using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides a convenient set of methods for instantiating view extensions.
    /// </summary>
    public static class ViewFactory
    {
        /// <summary>
        /// Creates a view extension that matches the specified GUI toolkit.
        /// </summary>
        /// <param name="extensionPoint">The view extension point.</param>
        /// <param name="toolkitID">The desired GUI toolkit</param>
        /// <returns>The view object that was created.</returns>
        /// <exception cref="NotSupportedException">A view extension matching the specified GUI toolkit does not exist.</exception>
        public static IView CreateView(IExtensionPoint extensionPoint, GuiToolkitID toolkitID)
        {
            // create an attribute representing the GUI toolkitID
            GuiToolkitAttribute toolkitAttr = new GuiToolkitAttribute(toolkitID);

            // create an extension that is tagged with the same toolkit
            return (IView)extensionPoint.CreateExtension(new AttributeExtensionFilter(toolkitAttr));
        }

        /// <summary>
        /// Creates a view extension that matches GUI toolkit of the main workstation view.
        /// </summary>
        /// <param name="extensionPoint">The view extension point.</param>
        /// <returns>The view object that was created.</returns>
        /// <exception cref="NotSupportedException">A view extension matching the GUI toolkit of the main view not exist.</exception>
        /// <exception cref="InvalidOperationException">The main workstation view has not yet been created.</exception>
        public static IView CreateView(IExtensionPoint extensionPoint)
        {
            IDesktopView desktopView = DesktopApplication.View;
            if (desktopView == null)
            {
                throw new InvalidOperationException(SR.ExceptionWorkstationViewNotCreated);
            }

            return CreateView(extensionPoint, desktopView.GuiToolkitID);
        }
    }
}
