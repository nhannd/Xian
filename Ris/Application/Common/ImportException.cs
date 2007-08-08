using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [Serializable]
    public class ImportException : Exception
    {
        public ImportException(string message)
            :base(message)
        {
        }

        public ImportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
