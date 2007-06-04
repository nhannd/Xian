using System;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Desktop.ExceptionPolicies
{
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
