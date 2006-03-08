
namespace ClearCanvas.Utilities.RebuildDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Text;

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
