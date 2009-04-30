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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a workspace within a desktop window.
    /// </summary>
    public class Workspace : DesktopObject, IWorkspace
	{
        #region Host Implementation

        // implements the host interface, which is exposed to the hosted application component
        private class Host : ApplicationComponentHost, IWorkspaceHost
        {
            private Workspace _workspace;

            internal Host(Workspace workspace, IApplicationComponent component)
                :base(component)
            {
				Platform.CheckForNullReference(workspace, "workspace");
                _workspace = workspace;
            }

            public override void Exit()
            {
                _workspace._exitRequestedByComponent = true;

                // close the workspace
                _workspace.Close(UserInteraction.Allowed, CloseReason.Program);
            }

            public override CommandHistory CommandHistory
            {
                get { return _workspace._commandHistory; }
            }

            public override DesktopWindow DesktopWindow
            {
                get { return _workspace._desktopWindow; }
            }

            public override string Title
            {
                get { return _workspace.Title; }
                set { _workspace.Title = value; }
            }

        	public bool IsWorkspaceActive
        	{
				get { return _workspace.Active; }
        	}

			public event EventHandler IsWorkspaceActiveChanged
			{
				add { _workspace.ActiveChanged += value; }
				remove { _workspace.ActiveChanged -= value; }
			}
        }

        #endregion


        private Host _host;
        private DesktopWindow _desktopWindow;
        private CommandHistory _commandHistory;
        private bool _exitRequestedByComponent;
        private bool _userClosable;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args">Arguments for creation of the <see cref="Workspace"/>.</param>
        /// <param name="desktopWindow">The <see cref="DesktopWindow"/> that owns the <see cref="Workspace"/>.</param>
        protected internal Workspace(WorkspaceCreationArgs args, DesktopWindow desktopWindow)
            : base(args)
        {
            _commandHistory = new CommandHistory(100);
            _desktopWindow = desktopWindow;
            _userClosable = args.UserClosable;

            _host = new Host(this, args.Component);
        }

        #region Public properties

        /// <summary>
        /// Gets the hosted component.
        /// </summary>
        public object Component
        {
            get { return _host.Component; }
        }

        /// <summary>
        /// Gets the desktop window that owns this workspace.
        /// </summary>
        public DesktopWindow DesktopWindow
        {
            get { return _desktopWindow; }
        }

        /// <summary>
        /// Gets the command history associated with this workspace.
        /// </summary>
        public CommandHistory CommandHistory
        {
            get { return _commandHistory; }
        }

        /// <summary>
        /// Gets a value indicating whether this workspace can be closed directly by the user.
        /// </summary>
        public bool UserClosable
        {
            get { return _userClosable; }
        }

        #endregion

        #region Protected overrides

    	/// <summary>
    	/// Asks the object whether it is in a closable state without user intervention.
    	/// </summary>
    	/// <returns>True if the object can be closed, otherwise false.</returns>
    	protected internal override bool CanClose()
        {
            return _exitRequestedByComponent || _host.Component.CanExit();
        }

        /// <summary>
        /// Gives the hosted component a chance to prepare for a forced exit.
        /// </summary>
        protected override bool PrepareClose(CloseReason reason)
        {
            base.PrepareClose(reason);

            return _exitRequestedByComponent || _host.Component.PrepareExit();
        }

        /// <summary>
        /// Overridden to prevent closing the <see cref="Workspace"/> if <see cref="UserClosable"/> is false.
        /// </summary>
        protected override void OnClosing(ClosingEventArgs args)
        {
            bool parentClosing = (args.Reason & CloseReason.ParentClosing) == CloseReason.ParentClosing;
            bool userClosing = (args.Reason & CloseReason.UserInterface) == CloseReason.UserInterface;

            if (userClosing && !parentClosing && !_userClosable)
            {
                // the user is attempting to close this workspace (not the parent window), but it is not user-closable
                // cancel the close, and do not call the base class (do not fire the event publicly)
                args.Cancel = true;

                return;
            }

            base.OnClosing(args);
        }

        /// <summary>
        /// Starts the hosted component.
        /// </summary>
        protected override void Initialize()
        {
            _host.StartComponent();
            base.Initialize();
        }

        /// <summary>
        /// Stops the hosted component.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _host != null)
            {
                _host.StopComponent();
                _host = null;
            	_desktopWindow = null;
            }
        }

        /// <summary>
        /// Creates a view for this workspace.
        /// </summary>
        protected sealed override IDesktopObjectView CreateView()
        {
            return _desktopWindow.CreateWorkspaceView(this);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the set of actions that are exported from the hosted component.
        /// </summary>
        protected internal IActionSet Actions
        {
            get { return _host.Component.ExportedActions; }
        }

        /// <summary>
        /// Gets the view for this object as an <see cref="IWorkspaceView"/>.
        /// </summary>
        protected IWorkspaceView WorkspaceView
        {
            get { return (IWorkspaceView)this.View; }
        }

        #endregion

        #region IWorkspace Members

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> that owns this <see cref="Workspace"/>.
		/// </summary>
        IDesktopWindow IWorkspace.DesktopWindow
        {
            get { return _desktopWindow; }
        }

        #endregion

    }
}
