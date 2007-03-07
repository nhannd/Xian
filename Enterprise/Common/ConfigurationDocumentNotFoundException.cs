using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    [Serializable]
    public class ConfigurationDocumentNotFoundException : Exception
    {
        public ConfigurationDocumentNotFoundException(string message)
            : base(message)
        {
        }

        public ConfigurationDocumentNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
   }
}
