using System;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Desktop.ExceptionPolicies
{
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
