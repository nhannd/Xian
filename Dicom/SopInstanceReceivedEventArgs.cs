namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class SopInstanceReceivedEventArgs : EventArgs
    {
        public SopInstanceReceivedEventArgs(string fileName)
        {
            _sopFileName = fileName;
        }

        public System.String GetSopFileName()
        {
            return _sopFileName;
        }

        private string _sopFileName;

    }
}
