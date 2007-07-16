using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class DesktopWindowCreationArgs : DesktopObjectCreationArgs
    {
        private string _toolbarSite;
        private string _menuSite;

        public DesktopWindowCreationArgs()
        {

        }

        public DesktopWindowCreationArgs(string title, string name)
            :base(title, name)
        {
        }

        public string ToolbarSite
        {
            get { return _toolbarSite; }
            set { _toolbarSite = value; }
        }

        public string MenuSite
        {
            get { return _menuSite; }
            set { _menuSite = value; }
        }
    }
}
