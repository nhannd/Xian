using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    /// <summary>
    /// Used by a service to indicate to the client that a request was rejected because it was invalid.  The client will likely
    /// display the contained message to the end user.  Therefore, the message should be appropriate for the end-user.
    /// </summary>
    [Serializable]
    public class RequestValidationException : Exception
    {
        public RequestValidationException(string message)
            : base(message)
        {
        }

        public RequestValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
   }
}
