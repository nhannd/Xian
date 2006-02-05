
namespace ClearCanvas.Dicom.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Application-level exceptions that are routed from lower-level DICOM library calls
    /// </summary>
    public class DicomException : System.ApplicationException
    {
        public DicomException(string message)
            : base(message)
        {
        }

        public DicomException(string message, System.Exception exception)
            : base(message, exception)
        {
        }
    }

    public class NetworkDicomException : DicomException
    {
        public NetworkDicomException(string message)
            : base(message)
        {
        }

        public NetworkDicomException(string message, System.Exception exception)
            : base(message, exception)
        {
        }
    }
}
