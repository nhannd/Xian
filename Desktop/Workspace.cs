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
