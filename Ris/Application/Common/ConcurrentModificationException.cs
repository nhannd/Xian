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

    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(ConcurrentModificationException))]
    [ExceptionPolicyFor(typeof(FaultException<ConcurrentModificationException>))]
    public class ConcurrentModificationExceptionPolicy : ExceptionPolicyBase
    {
        public override ExceptionReport Handle(Exception e, string userMessage)
        {
            return base.Handle(e, userMessage);
        }
    }
}
