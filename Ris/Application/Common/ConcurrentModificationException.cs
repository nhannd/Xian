using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Common;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common
{
    [Serializable]
    public class ConcurrentModificationException : Exception
    {
        public ConcurrentModificationException(Exception inner)
            : base(SR.ExceptionConcurrentModification, inner)
        {
        }

        public ConcurrentModificationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
