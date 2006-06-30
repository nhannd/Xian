using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    // intentionally left off the [ExtensionPoint] attribute - do not actually want to expose this to framework
    // it is just a dummy class to make this work for now
    public class StubToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public class ApplicationComponentHostWorkspace : Workspace
    {
        // this is just a dummy tool context to make this work for now
        public class WorkspaceToolContext : ToolContext
        {
            public WorkspaceToolContext()
                : base(new StubToolExtensionPoint())
            {
            }
        }

        // implements the host interface, which is exposed to the hosted application component
        class Host : IApplicationComponentHost
        {
            private ApplicationComponentHostWorkspace _workspace;

            internal Host(ApplicationComponentHostWorkspace workspace)
            {
                _workspace = workspace;
            }
        }

        private IApplicationComponent _component;
        private IExtensionPoint _componentViewExtPoint;
        private ApplicationComponentHostWorkspaceView _view;
        private ToolSet _toolSet;


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
            get
            {
                if (_toolSet == null)
                {
                    _toolSet = new ToolSet(new WorkspaceToolContext());
                }
                return _toolSet;
            }
        }
    }
}
