using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using System.ServiceModel.Security;

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

            // report to user
            context.ShowMessageBox(e.Message, true);
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

            // report to user
            context.ShowMessageBox(SR.MessageConcurrentModification, true);

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
            // log this as an error
            context.Log(e, LogLevel.Error);

            // report to user
            context.ShowMessageBox(SR.MessageTimeout, true);
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
            context.ShowMessageBox(SR.MessageCommunicationError, true);
        }
    }

    /// <summary>
    /// Policy for Security Access exceptions
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(SecurityAccessDeniedException))]
    public class SecurityAccessDeniedExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            context.Log(e, LogLevel.Debug);
            context.ShowMessageBox(SR.MessageAccessDenied, true);

            // this exception is not recoverable
            // (well, technically it is, but we don't want to encourage retries !!!)
            context.Abort();
        }
    }



}
