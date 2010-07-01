#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using Castle.Core.Interceptor;
using System.ServiceModel;
using ClearCanvas.Common;

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
				if (!IsFailoverApplicable(e))
					throw;

				// failover is applicable, so log failure before we attempt failover
				var channel = invocation.InvocationTarget;
				LogFailure(invocation, e, ((IClientChannel)channel).RemoteAddress);

				// attempt failover
				if (!DoFailover(invocation))
					throw;
			}
		}

		private bool DoFailover(IInvocation invocation)
		{
			object channel;

			// loop until we find a channel that succeeds or run out of failover channels
			while ((channel = GetFailoverChannel(invocation)) != null)
			{
				var remoteEndpoint = ((IClientChannel)channel).RemoteAddress;
				LogAttempt(invocation, remoteEndpoint);
				try
				{
					// try again using this channel, being sure to dispose of it
					using (channel as IDisposable)
					{
						var retVal = invocation.Method.Invoke(channel, invocation.Arguments);

						// success!
						invocation.ReturnValue = retVal;
						return true;
					}
				}
				catch (Exception ex)
				{
					// try next failover channel
					LogFailure(invocation, ex, remoteEndpoint);
				}
			}

			// ran out of failover channels without success
			return false;
		}

		private object GetFailoverChannel(IInvocation invocation)
		{
			return _serviceProvider.GetFailoverChannel((IClientChannel)invocation.InvocationTarget);
		}

		private static void LogFailure(IInvocation invocation, Exception e, EndpointAddress failedEndpoint)
		{
			Platform.Log(LogLevel.Error, e,
				"Service operation {0} failed on endpoint {1} with specified exception.",
				invocation.Method.Name,
				failedEndpoint.Uri);
		}

		private static void LogAttempt(IInvocation invocation, EndpointAddress attemptEndpoint)
		{
			Platform.Log(LogLevel.Error,
				"Attempting to retry service operation {0} on endpoint {1}.",
				invocation.Method.Name,
				attemptEndpoint.Uri);
		}

		private static bool IsFailoverApplicable(Exception e)
		{
			// presumably this is a communication timeout exception
			if (e is TimeoutException)
				return true;

			// communication exceptions should prompt fail over
			// but must ignore FaultException (which is a subclass of communication exception)
			if (e is CommunicationException && !(e is FaultException))
				return true;

			// any other exception should not prompt fail over
			return false;
		}

		#endregion
	}
}
