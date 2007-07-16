using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class WorkspaceCreationArgs : DesktopObjectCreationArgs
    {
        private IApplicationComponent _component;

        public WorkspaceCreationArgs()
        {

        }

        public WorkspaceCreationArgs(IApplicationComponent component, string title, string name)
            :base(title, name)
        {
            _component = component;
        }

        public IApplicationComponent Component
        {
            get { return _component; }
            set { _component = value; }
        }
    }
}
