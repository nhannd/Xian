using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using System.ServiceModel;
using ClearCanvas.Common;
using System.Collections;

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

		public object Intercept(IInvocation invocation, params object[] args)
		{
            try
            {
                // attempt to call the default service object
                return invocation.Proceed(args);
            }
            catch (Exception e)
            {
                // rethrow if we can't fail-over on this exception
                ThrowIfFailoverNotApplicable(e);

                // attempt failover
                return DoFailover(invocation, args, e);
            }
		}

        private object DoFailover(IInvocation invocation, object[] args, Exception e)
        {
            object channel = invocation.InvocationTarget;
            EndpointAddress remoteEndpoint = (channel as IClientChannel).RemoteAddress;

            // log the exception
            Platform.Log(LogLevel.Error, e,
                "Service operation {0} failed on endpoint {1} with specified exception, attempting failover.",
                invocation.Method.Name,
                remoteEndpoint.Uri);

            // loop until we find a channel that succeeds or run out of failover channels
            while ((channel = GetFailoverChannel(invocation, remoteEndpoint))
                != null)
            {
                remoteEndpoint = (channel as IClientChannel).RemoteAddress;
                try
                {
                    // try again using this channel, being sure to dispose of it
                    using (channel as IDisposable)
                    {
                        return invocation.Method.Invoke(channel, args);
                    }
                }
                catch (Exception ex)
                {
                    // log and try next failover channel
                    // log the exception
                    Platform.Log(LogLevel.Error, ex,
                        "Service operation {0} failed on endpoint {1} with specified exception, attempting failover.",
                        invocation.Method.Name,
                        remoteEndpoint.Uri);
                }
            }

            // ran out of fail over channels without success
            throw e;
        }

        private void ThrowIfFailoverNotApplicable(Exception e)
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

        private object GetFailoverChannel(IInvocation invocation, EndpointAddress failedEndpoint)
        {
            return _serviceProvider.GetFailoverChannel(
              invocation.Method.DeclaringType, failedEndpoint);
        }

		#endregion
	}
}
