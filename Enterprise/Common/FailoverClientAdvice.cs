#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Client-side advice to implement transparent failover.
    /// </summary>
    /// <remarks>
    /// If the invocation fails due to a <see cref="CommunicationException"/> (excluding
    /// <see cref="FaultException"/>s) or <see cref="TimeoutException"/>, this interceptor
    /// will attempt to obtain an alternate service channel and retry the service operation
    /// on the alternate channel.  The process is repeated until the operation succeeds or
    /// there are no more alternate channels to try.
    /// </remarks>
    class FailoverClientAdvice : IInterceptor
	{
		private readonly RemoteServiceProviderBase _serviceProvider;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceProvider">Service provider instance that provides the failover channel.</param>
		public FailoverClientAdvice(RemoteServiceProviderBase serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
            try
            {
                // attempt to call the default service object
                invocation.Proceed();
            }
            catch (Exception e)
            {
                // rethrow if we can't fail-over on this exception
                ThrowIfFailoverNotApplicable(e);

                // attempt failover
                DoFailover(invocation, e);
            }
		}

        private object DoFailover(IInvocation invocation, Exception e)
        {
            object channel = invocation.InvocationTarget;
            EndpointAddress remoteEndpoint = ((IClientChannel) channel).RemoteAddress;

            // log the exception
            LogFailure(invocation, e, remoteEndpoint);

            // loop until we find a channel that succeeds or run out of failover channels
            while ((channel = GetFailoverChannel(invocation, remoteEndpoint))
                != null)
            {
                remoteEndpoint = ((IClientChannel) channel).RemoteAddress;
                try
                {
                    // try again using this channel, being sure to dispose of it
                    using (channel as IDisposable)
                    {
                        invocation.Method.Invoke(channel, invocation.Arguments);
                    }
                }
                catch (Exception ex)
                {
                    // log and try next failover channel
                    LogFailure(invocation, ex, remoteEndpoint);
                }
            }

            // ran out of failover channels without success
            throw e;
        }

        private object GetFailoverChannel(IInvocation invocation, EndpointAddress failedEndpoint)
        {
        	var proxy = (IRemoteServiceProxy)invocation.Proxy;
            return _serviceProvider.GetFailoverChannel(
			  proxy.ServiceContract, failedEndpoint);
        }

        private static void LogFailure(IInvocation invocation, Exception e, EndpointAddress failedEndpoint)
        {
            Platform.Log(LogLevel.Error, e,
                "Service operation {0} failed on endpoint {1} with specified exception, attempting failover.",
                invocation.Method.Name,
                failedEndpoint.Uri);
        }

        private static void ThrowIfFailoverNotApplicable(Exception e)
        {
            // must ignore FaultException, otherwise it will 
            // be treated as a CommunicationException
            if (e is FaultException)
                throw e;

            // these exceptions should prompt fail over
            if (e is CommunicationException || e is TimeoutException)
                return;

            // any other exception should be re-thrown
            throw e;
        }

		#endregion
	}
}
