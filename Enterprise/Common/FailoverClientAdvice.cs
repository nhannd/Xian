using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using System.ServiceModel;
using ClearCanvas.Common;

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
			catch(CommunicationException e)
			{
				// log the exception
				Platform.Log(LogLevel.Error, e);

				// try again using the fail-over
				// TODO what if there is no failover specified?
				object channel = _serviceProvider.CreateChannel(invocation.Method.DeclaringType, true);
				return invocation.Method.Invoke(channel, args);
			}
		}

		#endregion
	}
}
