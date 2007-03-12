using System;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [Serializable]
    public class HL7ProcessingException : Exception
    {
        public HL7ProcessingException(Exception inner)
            : base(SR.ExceptionHL7Processing, inner)
        {
        }

        public HL7ProcessingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }
}
