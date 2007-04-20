using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Common
{
    /// <summary>
    /// Used by a service to indicate to the client that a request was rejected because it was invalid.  The client will likely
    /// display the contained message to the end user.  Therefore, the message should be appropriate for the end-user.
    /// </summary>
    [Serializable]
    public class RequestValidationException : Exception
    {
        public RequestValidationException(string message)
            : base(message)
        {
        }

        public RequestValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(RequestValidationException))]
    [ExceptionPolicyFor(typeof(FaultException<RequestValidationException>))]
    public class RequestValidationExceptionPolicy : ExceptionPolicyBase
    {
        public override ExceptionReport Handle(Exception e, string userMessage)
        {
            string message = string.IsNullOrEmpty(userMessage) ? e.Message : userMessage;
            message = message + " (Handled by RequestValidationPolicy)";

            return new ExceptionReport(message, ExceptionReportAction.ReportInDialog);
        }
    }
}
