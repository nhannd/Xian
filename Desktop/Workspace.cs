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

            public override void SetTitle(string title)
            {
                _workspace.Title = title;
            }
        }

        #endregion


        private Host _host;
        private DesktopWindow _desktopWindow;
        private CommandHistory _commandHistory;
        private bool _exitRequestedByComponent;


        protected internal Workspace(WorkspaceCreationArgs args, DesktopWindow desktopWindow)
            : base(args)
        {
            _commandHistory = new CommandHistory(100);
            _desktopWindow = desktopWindow;

            _host = new Host(this, args.Component);
        }

        #region Public properties

        /// <summary>
        /// Gets the hosted component
        /// </summary>
        public IApplicationComponent Component
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

        #endregion

        #region Protected overrides

        protected internal override bool CanClose(UserInteraction interactive)
        {
            return _exitRequestedByComponent || _host.Component.CanExit(interactive);
        }

        protected override void Initialize()
        {
            _host.StartComponent();
            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _host != null)
            {
                _host.StopComponent();
                _host = null;
            }
        }

        protected override IDesktopObjectView CreateView()
        {
            return _desktopWindow.CreateWorkspaceView(this);
        }

        #endregion

        #region Helpers

        protected internal IActionSet Actions
        {
            get { return _host.Component.ExportedActions; }
        }

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
