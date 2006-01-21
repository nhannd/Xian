
namespace ClearCanvas.Dicom.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

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
