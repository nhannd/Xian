using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class ShelfCreationArgs : DesktopObjectCreationArgs
    {
        private IApplicationComponent _component;
        private ShelfDisplayHint _displayHint;

        public ShelfCreationArgs()
        {

        }

        public ShelfCreationArgs(IApplicationComponent component, string title, string name, ShelfDisplayHint displayHint)
            : base(title, name)
        {
            _component = component;
            _displayHint = displayHint;
        }

        public ShelfCreationArgs(IApplicationComponent component, string title, string name)
            : this(component, name, title, ShelfDisplayHint.None)
        {
        }

        public IApplicationComponent Component
        {
            get { return _component; }
            set { _component = value; }
        }

        public ShelfDisplayHint DisplayHint
        {
            get { return _displayHint; }
            set { _displayHint = value; }
        }
    }
}
