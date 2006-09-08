using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class ServerPath
    {
        public ServerPath(String pathname, String description)
        {
            _pathname = pathname;
            _description = description;
        }

        private String _pathname;
        private String _description;

        public String Pathname
        {
            get { return _pathname; }
            set { _pathname = value; }
        }

        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

    }
}
