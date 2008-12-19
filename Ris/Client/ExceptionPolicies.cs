#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Security;
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
            context.Log(LogLevel.Debug, e);

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
            context.Log(LogLevel.Debug, e);

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
            context.Log(LogLevel.Error, e);

            // report to user
            context.ShowMessageBox(SR.MessageTimeout, true);
        }
    }

    /// <summary>
    /// Policy for WCF communication exceptions
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(CommunicationException))]
    [ExceptionPolicyFor(typeof(EndpointNotFoundException))]
    public class CommunicationExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext context)
        {
            context.Log(LogLevel.Error, e);
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
            context.Log(LogLevel.Debug, e);
            context.ShowMessageBox(SR.MessageAccessDenied, true);

            // this exception is not recoverable
            // (well, technically it is, but we don't want to encourage retries !!!)
            context.Abort();
        }
    }

    /// <summary>
    /// Policy for <see cref="MessageSecurityException"/>.
    /// </summary>
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    [ExceptionPolicyFor(typeof(MessageSecurityException))]
    public class MessageSecurityExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext exceptonHandlingContext)
        {
            // typically this means authentication failed, which is usually because
            // the session has expired
            SessionManager.RenewLogin();
        }
    }
}
