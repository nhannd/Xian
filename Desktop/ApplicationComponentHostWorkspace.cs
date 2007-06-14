using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Define an extension point for a view onto this workspace
    /// </summary>
    [ExtensionPoint]
    public class ApplicationComponentHostWorkspaceViewExtensionPoint : ExtensionPoint<IWorkspaceView>
    {
    }

    /// <summary>
    /// Hosts an application component in a workspace.  See <see cref="ApplicationComponent.LaunchAsWorkspace"/>.
    /// </summary>
    [AssociateView(typeof(ApplicationComponentHostWorkspaceViewExtensionPoint))]
    public class ApplicationComponentHostWorkspace : Workspace
    {
        // implements the host interface, which is exposed to the hosted application component
        class Host : ApplicationComponentHost
        {
            private ApplicationComponentHostWorkspace _workspace;

            internal Host(ApplicationComponentHostWorkspace workspace, IApplicationComponent component, ApplicationComponentExitDelegate exitCallback)
                :base(component, exitCallback)
            {
				Platform.CheckForNullReference(workspace, "workspace");
                _workspace = workspace;
            }

            public override void Exit()
            {
                _workspace._exitRequestedByComponent = true;

                // close the workspace
                _workspace.DesktopWindow.WorkspaceManager.Workspaces.Remove(_workspace);
            }

            public override CommandHistory CommandHistory
            {
                get { return _workspace.CommandHistory; }
            }

            public override IDesktopWindow DesktopWindow
            {
                get { return _workspace.DesktopWindow; }
            }

            public override void SetTitle(string title)
            {
                _workspace.Title = title;
            }
        }

        private Host _host;
        private bool _exitRequestedByComponent;


        public ApplicationComponentHostWorkspace(
            IDesktopWindow desktopWindow,
            IApplicationComponent component,
            string title,
            ApplicationComponentExitDelegate exitCallback)
            :base(title, desktopWindow)
        {
			Platform.CheckForNullReference(component, "component");

            _host = new Host(this, component, exitCallback);
            _host.StartComponent();
        }

        /// <summary>
        /// Gets the hosted component
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _host.Component; }
        }

        #region IWorkspace Members

        public override bool CanClose()
        {
            return _exitRequestedByComponent || _host.Component.CanExit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _host != null)
            {
                _host.StopComponent();
                _host = null;
            }
        }

        public override IActionSet Actions
        {
            get { return _host.Component.ExportedActions; }
        }

        #endregion
    }
}
