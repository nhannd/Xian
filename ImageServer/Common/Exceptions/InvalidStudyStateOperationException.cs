using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when the study state is invalid for the operation.
    /// </summary>
    public class InvalidStudyStateOperationException : System.Exception
    {
        public InvalidStudyStateOperationException(string message):base(message)
        {
            
        }
    }
}
