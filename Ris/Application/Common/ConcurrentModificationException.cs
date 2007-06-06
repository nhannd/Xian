using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [Serializable]
    public class ConcurrentModificationException : Exception
    {
        public ConcurrentModificationException(string message)
            : base(SR.ExceptionConcurrentModification + " : " + message)
        {
        }

        public ConcurrentModificationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
