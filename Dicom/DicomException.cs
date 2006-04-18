
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
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GeneralDicomException() { }

        /// <summary>
        /// Constructor that takes a message string.
        /// </summary>
        /// <param name="message">Message that will be accessible to the user or client describing the
        /// nature of the exception.</param>
        public GeneralDicomException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor that takes in a message string and an inner exception.
        /// </summary>
        /// <param name="message">Message that explains the nature of the exception.</param>
        /// <param name="exception">The nested, or inner exception that is relevant.</param>
        public GeneralDicomException(string message, Exception exception)
            : base(message, exception)
        {
        }

        protected GeneralDicomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }

    /// <summary>
    /// Application-level exceptions that are routed from the lower-level DICOM libary, specific to
    /// functionality related to networking, such as Query'ing and Retrieve'ing.
    /// </summary>
    [Serializable]
    public class NetworkDicomException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NetworkDicomException() { }

        /// <summary>
        /// Constructor that takes a message string.
        /// </summary>
        /// <param name="message">Message that will be accessible to the user or client describing the
        /// nature of the exception.</param>
        public NetworkDicomException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor that takes in a message string and an inner exception.
        /// </summary>
        /// <param name="message">Message that explains the nature of the exception.</param>
        /// <param name="exception">The nested, or inner exception that is relevant.</param>
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
