using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    // intentionally left off the [Extension] attribute - do not actually want to expose this to framework
    // it is just a dummy class to make this work for now
    public class StubToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public class ApplicationComponentHostWorkspace : Workspace, IApplicationComponentHost
    {
        public class WorkspaceToolContext : ToolContext
        {
            public WorkspaceToolContext()
                : base(new StubToolExtensionPoint())
            {
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

            _component.SetHost(this);
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

        protected override ToolContext CreateToolContext()
        {
            return new WorkspaceToolContext();
        }
    }
}
