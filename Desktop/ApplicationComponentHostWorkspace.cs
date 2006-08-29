using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    [ExtensionPoint]
    public class ApplicationComponentHostWorkspaceViewExtensionPoint : ExtensionPoint<IWorkspaceView>
    {
    }

    [AssociateView(typeof(ApplicationComponentHostWorkspaceViewExtensionPoint))]
    public class ApplicationComponentHostWorkspace : Workspace
    {
        // implements the host interface, which is exposed to the hosted application component
        class Host : IApplicationComponentHost
        {
            private ApplicationComponentHostWorkspace _workspace;

            internal Host(ApplicationComponentHostWorkspace workspace)
            {
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
            _component = component;
            _exitCallback = exitCallback;

            _component.SetHost(new Host(this));
        }

        public override void Initialize(IDesktopWindow desktopWindow)
        {
            base.Initialize(desktopWindow);
            _component.Start();
        }

        public IApplicationComponent Component
        {
            get { return _component; }
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
    }
}
