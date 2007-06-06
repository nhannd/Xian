using System;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(ConcurrentModificationException))]
    [ExceptionPolicyFor(typeof(FaultException<ConcurrentModificationException>))]
    public class ConcurrentModificationExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            string message =
                string.IsNullOrEmpty(context.UserMessage) == false
                    ? string.Format("{0}: {1}", context.UserMessage, SR.MessageConcurrentModification)
                    : "This item was updated by another user.  Changes will be lost.";

            context.ShowMessageBox(message);
            context.Abort();
        }
    }
}