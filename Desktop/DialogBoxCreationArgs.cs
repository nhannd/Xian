using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class DialogBoxCreationArgs : DesktopObjectCreationArgs
    {
        private IApplicationComponent _component;

        public DialogBoxCreationArgs()
        {

        }

        public DialogBoxCreationArgs(IApplicationComponent component, string title, string name)
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
