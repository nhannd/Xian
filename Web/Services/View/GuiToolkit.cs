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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Web.Services.View
{
    /// <summary>
    /// WinForms implementation of <see cref="IGuiToolkit"/>. 
    /// </summary>
    [ExtensionOf(typeof(GuiToolkitExtensionPoint))]
    public class GuiToolkit : IGuiToolkit
    {
        private event EventHandler _started;

        #region IGuiToolkit Members

        /// <summary>
        /// Gets the toolkit ID.
        /// </summary>
        /// <value>Always returns <see cref="ClearCanvas.Common.GuiToolkitID.WinForms"/>.</value>
        public string ToolkitID
        {
            get { return GuiToolkitID.Web; }
        }

        /// <summary>
        /// Occurs after the message loop has started.
        /// </summary>
        public event EventHandler Started
        {
            add { _started += value; }
            remove { _started -= value; }
        }

        /// <summary>
        /// Initializes WinForms and starts the message loop.  Blocks until <see cref="Terminate"/> is called.
        /// </summary>
        public void Run()
        {
            EventsHelper.Fire(_started, this, EventArgs.Empty);
        }

        /// <summary>
        /// Terminates the message loop.
        /// </summary>
        public void Terminate()
        {
        }

        #endregion
    }
}
