namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Argument type that encapsulates data pertinent to a
    /// SOP Instance having been received.
    /// </summary>
    public class SopInstanceReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="fileName">The path and filename of the newly stored SOP Instance.</param>
        public SopInstanceReceivedEventArgs(string fileName)
        {
            _sopFileName = fileName;
        }

        /// <summary>
        /// Gets the path and file name of the newly stored SOP Instance.
        /// </summary>
        /// <returns>Path and file name of stored SOP Instance.</returns>
        public System.String GetSopFileName()
        {
            return _sopFileName;
        }

        private string _sopFileName;

    }
}
