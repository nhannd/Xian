using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [Serializable]
    public class EnablementMethodNotFoundException : Exception
    {
        public EnablementMethodNotFoundException(string enablementMethodName, string methodName)
            : base(String.Format(SR.ExceptionEnablementMethodNotFound, enablementMethodName, methodName))
        {
        }

        public EnablementMethodNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
