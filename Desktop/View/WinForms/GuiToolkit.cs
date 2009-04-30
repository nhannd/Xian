#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
