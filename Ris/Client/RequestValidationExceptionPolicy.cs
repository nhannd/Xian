using System;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(RequestValidationException))]
    [ExceptionPolicyFor(typeof(FaultException<RequestValidationException>))]
    public class RequestValidationExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            string message = 
                string.IsNullOrEmpty(context.UserMessage) == false
                    ? string.Format("{0}: {1}", context.UserMessage, e.Message)
                    : e.Message;

            context.ShowMessageBox(message);
        }
    }
}