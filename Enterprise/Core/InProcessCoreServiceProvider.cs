#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// This service provider allows the application server to make use of core services internally
	/// by providing these services in-process.
	/// </summary>
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	public class InProcessCoreServiceProvider : IServiceProvider
	{
		private readonly IServiceFactory _serviceFactory;

		public InProcessCoreServiceProvider()
		{
			_serviceFactory = new ServiceFactory(new CoreServiceExtensionPoint());

			// exception logging occurs outside of the main persistence context
			// JR: is there any point in logging exceptions from the in-process provider?  Or is this just redundant?
			//_serviceFactory.Interceptors.Add(new ExceptionLoggingAdvice());

			// add audit advice outside of main persistence context advice, so that messages are written post-commit
			_serviceFactory.Interceptors.Add(new AuditAdvice());

			// add persistence context advice, that controls the persistence context for the main transaction
			_serviceFactory.Interceptors.Add(new PersistenceContextAdvice());
		}

		#region IServiceProvider Members

		public object GetService(Type serviceType)
		{
			if (_serviceFactory.HasService(serviceType))
			{
				return _serviceFactory.GetService(serviceType);
			}
			return null;    // as per MSDN
		}

		#endregion
	}
}
