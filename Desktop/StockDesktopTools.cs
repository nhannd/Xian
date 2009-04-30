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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Implementation of standard desktop tools.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
    public class StockDesktopTools
    {
		/// <summary>
		/// Closes the <see	cref="IDesktopWindow"/> that owns this tool.
		/// </summary>
		/// <remarks>
		/// For internal framework use only.
		/// </remarks>
		[MenuAction("exit", "global-menus/MenuFile/MenuCloseWindow", "CloseWindow", KeyStroke = XKeys.Alt | XKeys.F4)]
		[GroupHint("exit", "Application.Exit")]
		[ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
        public class CloseWindowTool : Tool<IDesktopToolContext>
        {
			/// <summary>
			/// Constructor.
			/// </summary>
            public CloseWindowTool()
            {
            }
			
			/// <summary>
			/// Closes the <see	cref="IDesktopWindow"/> that owns this tool.
			/// </summary>
			public void CloseWindow()
			{
                this.Context.DesktopWindow.Close(UserInteraction.Allowed, CloseReason.UserInterface);
			}
        }

		/// <summary>
		/// Closes the active <see cref="IWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// For internal framework use only.
		/// </remarks>
		[MenuAction("closeWorkspace", "global-menus/MenuFile/MenuCloseWorkspace", "CloseWorkspace", KeyStroke = XKeys.Control | XKeys.F4)]
        [EnabledStateObserver("closeWorkspace", "Enabled", "EnabledChanged")]
		[GroupHint("closeWorkspace", "Application.Workspace.Close")]
        [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
        public class CloseWorkspaceTool : Tool<IDesktopToolContext>
        {
            private event EventHandler _enabledChanged;
            
			/// <summary>
			/// Constructor.
			/// </summary>
            public CloseWorkspaceTool()
            {
            }

			/// <summary>
			/// Disposes of this object; override to do custom cleanup.
			/// </summary>
			/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
			protected override void Dispose(bool disposing)
			{
				this.Context.DesktopWindow.Workspaces.ItemOpened -= WorkspacesChanged;
				this.Context.DesktopWindow.Workspaces.ItemClosed -= WorkspacesChanged;
				this.Context.DesktopWindow.Workspaces.ItemActivationChanged -= WorkspacesChanged;

				base.Dispose(disposing);
			}

			///<summary>
			/// Called by the framework to allow the tool to initialize itself.  This method will
			/// be called after <see cref="!:SetContext" /> has been called, which guarantees that 
			/// the tool will have access to its context when this method is called.
			///</summary>
			public override void Initialize()
            {
                base.Initialize();

                this.Context.DesktopWindow.Workspaces.ItemOpened += WorkspacesChanged;
                this.Context.DesktopWindow.Workspaces.ItemClosed += WorkspacesChanged;
                this.Context.DesktopWindow.Workspaces.ItemActivationChanged += WorkspacesChanged;
            }
			
			/// <summary>
			/// Closes the active <see cref="IWorkspace"/>.
			/// </summary>
            public void CloseWorkspace()
            {
                IDesktopWindow window = this.Context.DesktopWindow;
                IWorkspace activeWorkspace = window.ActiveWorkspace;
                if (activeWorkspace != null)
                {
                    activeWorkspace.Close();
                }
            }

			/// <summary>
			/// Gets the enabled state of the tool.
			/// </summary>
            public bool Enabled
            {
                get
                {
                    return this.Context.DesktopWindow.Workspaces.Count > 0 
                        && this.Context.DesktopWindow.ActiveWorkspace.UserClosable;
                }
            }

			/// <summary>
			/// Raised when the <see cref="Enabled"/> property has changed.
			/// </summary>
            public event EventHandler EnabledChanged
            {
                add { _enabledChanged += value; }
                remove { _enabledChanged -= value; }
            }

            private void WorkspacesChanged(object sender, ItemEventArgs<Workspace> e)
            {
                EventsHelper.Fire(_enabledChanged, this, new EventArgs());
            }
        }
    }
}
