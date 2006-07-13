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

            public void Exit()
            {
                // close the workspace
                // pass true, because the component requested the close, therefore there is no need
                // to call _component.CanClose()
                _workspace.Close(true);
            }

            public MessageBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
            {
                return Platform.ShowMessageBox(message, buttons);
            }

        }

        private IApplicationComponent _component;
        private IExtensionPoint _componentViewExtPoint;
        private ApplicationComponentHostWorkspaceView _view;
        private ApplicationComponentExitDelegate _exitCallback;


        internal ApplicationComponentHostWorkspace(string title, IApplicationComponent component,
            IExtensionPoint componentViewExtPoint, ApplicationComponentExitDelegate exitCallback)
            :base(title)
        {
            _component = component;
            _componentViewExtPoint = componentViewExtPoint;
            _exitCallback = exitCallback;

            _component.SetHost(new Host(this));
            _component.Start();
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

        public override void Close()
        {
            // try closing, but don't force the component to close
            Close(false);
        }

        protected void Close(bool force)
        {
            if (force || _component.CanExit())
            {
                // calling the base class will cause the workspace to close
                base.Close();

                if (_exitCallback != null)
                {
                    _exitCallback(_component);
                }
            }
        }

        public override void Cleanup()
        {
            _component.Stop();
        }

        public override IToolSet ToolSet
        {
            get { return _component.ToolSet; }
        }
    }
}
