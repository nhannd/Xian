using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
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

            public void Complete()
            {
                DesktopApplication.WorkspaceManager.Workspaces.Remove(_workspace);
            }
        }

        private IApplicationComponent _component;
        private IExtensionPoint _componentViewExtPoint;
        private ApplicationComponentHostWorkspaceView _view;


        public ApplicationComponentHostWorkspace(string title, IApplicationComponent component, IExtensionPoint componentViewExtPoint)
            :base(title)
        {
            _component = component;
            _componentViewExtPoint = componentViewExtPoint;

            _component.SetHost(new Host(this));
        }


        public override IWorkspaceView View
        {
            get
            {
                if (_view == null)
                {
                    IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateView(_componentViewExtPoint);
                    componentView.SetComponent(_component);
                    _view = new ApplicationComponentHostWorkspaceView(this, componentView);
                }
                return _view;
            }
        }

        public override void Cleanup()
        {
            // nothing to do
        }

        public override IToolSet ToolSet
        {
            get { return _component.ToolSet; }
        }
    }
}
