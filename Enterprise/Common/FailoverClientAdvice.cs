using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using System.ServiceModel;
using ClearCanvas.Common;
using System.Collections;

namespace ClearCanvas.Enterprise.Common
{
	class FailoverClientAdvice : IInterceptor
	{
		private readonly RemoteServiceProviderBase _serviceProvider;

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
                return Failover(invocation, args, e);
            }
		}

        private object Failover(IInvocation invocation, object[] args, Exception e)
        {
            // log the exception
            Platform.Log(LogLevel.Error, e, "Service operation failed with specified exception, attempting failover.");
            
            object channel = invocation.InvocationTarget;
            EndpointAddress remoteEndpoint = (channel as IClientChannel).RemoteAddress;

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
                    Platform.Log(LogLevel.Error, ex);
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
