using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Policy for exceptions that occur when the RIS server rejects a request because the input
    /// does not validate.
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(RequestValidationException))]
    [ExceptionPolicyFor(typeof(FaultException<RequestValidationException>))]
    public class RequestValidationExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            // only log this if debugging
            context.Log(e, LogLevel.Debug);

            string message =
                string.IsNullOrEmpty(context.UserMessage) == false
                    ? string.Format("{0}: {1}", context.UserMessage, e.Message)
                    : e.Message;

            context.ShowMessageBox(message);
        }
    }

    /// <summary>
    /// Policy for exceptions that occur when the RIS server rejects a request because it would modify
    /// objects that have been concurrently modified.
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(ConcurrentModificationException))]
    [ExceptionPolicyFor(typeof(FaultException<ConcurrentModificationException>))]
    public class ConcurrentModificationExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            // only log this if debugging
            context.Log(e, LogLevel.Debug);

            string message =
                string.IsNullOrEmpty(context.UserMessage) == false
                    ? string.Format("{0}: {1}", context.UserMessage, SR.MessageConcurrentModification)
                    : SR.MessageConcurrentModification;

            context.ShowMessageBox(message);

            // this exception is not recoverable
            context.Abort();
        }
    }

    /// <summary>
    /// Policy for exceptions that occur when a request times out
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(TimeoutException))]
    public class TimeoutExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            context.Log(e, LogLevel.Error);
            context.ShowMessageBox(SR.MessageTimeout);
        }
    }

    /// <summary>
    /// Policy for WCF communication exceptions
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(CommunicationException))]
    public class CommunicationExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            context.Log(e, LogLevel.Error);
            context.ShowMessageBox(SR.MessageCommunicationError);
        }
    }


}
