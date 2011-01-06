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

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IGuiToolkit"/>. 
    /// </summary>
    [ExtensionOf(typeof(GuiToolkitExtensionPoint))]
    public class GuiToolkit : IGuiToolkit
    {
        private event EventHandler _started;

        /// <summary>
        /// No-args constructor required by extension point framework.
        /// </summary>
        public GuiToolkit()
        {
            if (!Platform.IsWin32Platform)
                throw new NotSupportedException();
        }

        #region IGuiToolkit Members

        /// <summary>
        /// Gets the toolkit ID.
        /// </summary>
        /// <value>Always returns <see cref="ClearCanvas.Common.GuiToolkitID.WinForms"/>.</value>
        public string ToolkitID
        {
            get { return GuiToolkitID.WinForms; }
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
            // this must be called before any GUI objects are created - otherwise we get problems with icons not showing up
            System.Windows.Forms.Application.EnableVisualStyles();

            // create a timer to raise the Started event from the message pump
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += delegate(object sender, EventArgs args)
            {
                // immediately disable the timer so we don't get a second Tick
                timer.Dispose();
                EventsHelper.Fire(_started, this, EventArgs.Empty);
            };
            timer.Enabled = true;

            // start the message pump
            System.Windows.Forms.Application.Run();
        }

        /// <summary>
        /// Terminates the message loop.
        /// </summary>
        public void Terminate()
        {
            System.Windows.Forms.Application.Exit();
        }

        #endregion
    }
}
