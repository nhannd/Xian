using System;
using Castle.DynamicProxy;
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
            return _serviceProvider.GetFailoverChannel(
              invocation.Method.DeclaringType, failedEndpoint);
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
