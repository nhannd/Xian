
namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;

    /// <summary>
    /// Application-level exceptions that are routed from lower-level DICOM library calls
    /// </summary>
    [Serializable]
    public class GeneralDicomException : Exception
    {
        public GeneralDicomException() { }

        public GeneralDicomException(string message)
            : base(message)
        {
        }


        public GeneralDicomException(string message, System.Exception exception)
            : base(message, exception)
        {
        }

        protected GeneralDicomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }

    [Serializable]
    public class NetworkDicomException : Exception
    {
        public NetworkDicomException() { }

        public NetworkDicomException(string message)
            : base(message)
        {
        }


        public NetworkDicomException(string message, System.Exception exception)
            : base(message, exception)
        {
        }

        protected NetworkDicomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
