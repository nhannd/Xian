#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services.View
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
    public class ApplicationView : WebView, IApplicationView
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
            return DialogBoxAction.Ok;
        }

        #endregion

        #region Overrides of WebView

        public override void SetModelObject(object modelObject)
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            throw new NotImplementedException();
        }

        protected override Entity CreateEntity()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateEntity(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override void ProcessMessage(Message message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
