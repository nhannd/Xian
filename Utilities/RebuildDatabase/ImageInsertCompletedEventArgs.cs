using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Utilities.RebuildDatabase
{
    public class ImageInsertCompletedEventArgs : System.EventArgs
    {
        public ImageInsertCompletedEventArgs(String fileName)
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
