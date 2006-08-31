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
        class Host : IApplicationComponentHost
        {
            private ApplicationComponentHostWorkspace _workspace;

            internal Host(ApplicationComponentHostWorkspace workspace)
            {
				Platform.CheckForNullReference(workspace, "workspace");
                _workspace = workspace;
            }

            public void Exit()
            {
                // close the workspace
                _workspace.DesktopWindow.WorkspaceManager.Workspaces.Remove(_workspace);
            }

            public DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
            {
                return Platform.ShowMessageBox(message, buttons);
            }

            public CommandHistory CommandHistory
            {
                get { return _workspace.CommandHistory; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _workspace.DesktopWindow; }
            }
        }

        private IApplicationComponent _component;
        private ApplicationComponentExitDelegate _exitCallback;


        public ApplicationComponentHostWorkspace(
            IApplicationComponent component,
            string title,
            ApplicationComponentExitDelegate exitCallback)
            :base(title)
        {
			Platform.CheckForNullReference(component, "component");
            _component = component;
            _exitCallback = exitCallback;

            _component.SetHost(new Host(this));
        }

        /// <summary>
        /// Gets the hosted component
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
        }

        #region IWorkspace Members

        public override void Initialize(IDesktopWindow desktopWindow)
        {
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
			
			base.Initialize(desktopWindow);
            _component.Start();
        }

        public override bool CanClose()
        {
            return _component.CanExit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _component.Stop();

                if (_exitCallback != null)
                {
                    _exitCallback(_component);
                }
            }
        }

        public override IActionSet Actions
        {
            get { return _component.ExportedActions; }
        }

        #endregion
    }
}
