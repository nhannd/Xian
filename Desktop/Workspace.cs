#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
        class Host : ApplicationComponentHost
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
        }

        #endregion


        private Host _host;
        private DesktopWindow _desktopWindow;
        private CommandHistory _commandHistory;
        private bool _exitRequestedByComponent;
        private bool _userClosable;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        /// <param name="desktopWindow"></param>
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
        /// Gets or sets a value indicating whether this workspace can be closed directly by the user.
        /// </summary>
        public bool UserClosable
        {
            get { return _userClosable; }
            set { _userClosable = value; }
        }

        #endregion

        #region Protected overrides


        /// <summary>
        /// Checks if the hosted component can close.
        /// </summary>
        /// <param name="interactive"></param>
        /// <returns></returns>
        protected internal override bool CanClose(UserInteraction interactive)
        {
            return _exitRequestedByComponent || _host.Component.CanExit(interactive);
        }

        /// <summary>
        /// Overridden to prevent closing of workspace if <see cref="UserClosable"/> is false.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnClosing(ClosingEventArgs args)
        {
            if(args.Reason == CloseReason.UserInterface && !_userClosable)
            {
                // the user is attempting to close this workspace, but it is not user-closable
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
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _host != null)
            {
                _host.StopComponent();
                _host = null;
            }
        }

        /// <summary>
        /// Creates a view for this workspace.
        /// </summary>
        /// <returns></returns>
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

        IDesktopWindow IWorkspace.DesktopWindow
        {
            get { return _desktopWindow; }
        }

        #endregion

    }
}
