using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public abstract class DesktopObjectCreationArgs
    {
        private string _name;
        private string _title;

        protected DesktopObjectCreationArgs()
        {

        }

        protected DesktopObjectCreationArgs(string title, string name)
        {
            _name = name;
            _title = title;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
    }
}
