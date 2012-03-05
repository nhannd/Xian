using System;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Represents an exception thrown when an invalid dicom value is encountered by ImageServer Core Services
    /// </summary>
    public class InvalidDicomValueException : Exception
    {
        public InvalidDicomValueException(string message)
            : base(message)
        {

        }
    }
}