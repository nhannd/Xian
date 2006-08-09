using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Utilities.RebuildDatabase
{
    public class ImageInsertCompletingEventArgs : System.EventArgs
    {
        public ImageInsertCompletingEventArgs(String fileName)
        {
            _fileName = fileName;
        }

        public String FileName
        {
            get { return _fileName; }
        }

        private String _fileName;
    }
}
