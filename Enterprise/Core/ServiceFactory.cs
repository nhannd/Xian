#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Default implementation of <see cref="IServiceFactory"/>.  This implementation is thread-safe.
	/// </summary>
	public class ServiceFactory : IServiceFactory
	{
		private readonly ProxyGenerator _proxyGenerator;
		private readonly IExtensionPoint _serviceExtensionPoint;
		private readonly object _syncLock = new object();

		private readonly List<IInterceptor> _interceptors;

		/// <summary>
		/// Constructs a service factory that instantiates services based on the specified extension point.
		/// </summary>
		/// <param name="serviceExtensionPoint"></param>
		public ServiceFactory(IExtensionPoint serviceExtensionPoint)
		{
			_serviceExtensionPoint = serviceExtensionPoint;
			_proxyGenerator = new ProxyGenerator();
			_interceptors = new List<IInterceptor>();
		}

		public IList<IInterceptor> Interceptors
		{
			get { return _interceptors; }
		}

		/// <summary>
		/// Obtains an instance of the service that implements the specified contract.
		/// </summary>
		/// <typeparam name="TServiceContract"></typeparam>
		/// <returns></returns>
		public TServiceContract GetService<TServiceContract>()
		{
			return (TServiceContract)GetService(typeof(TServiceContract));
		}

		/// <summary>
		/// Obtains an instance of the service that implements the specified contract.
		/// </summary>
		/// <returns></returns>
		public object GetService(Type serviceContract)
		{
			lock (_syncLock)
			{
				// instantiate service object
				object service = _serviceExtensionPoint.CreateExtension(new TypeExtensionFilter(serviceContract));

				// note: _proxyGenerator does internal caching based on service contract
				// so subsequent calls based on the same contract will be fast
				var aopChain = new AopInterceptorChain(_interceptors);
				return _proxyGenerator.CreateInterfaceProxyWithTarget(serviceContract, service, aopChain);
			}
		}

		/// <summary>
		/// Lists the service classes that provide implementations of the contracts supported by this factory.
		/// </summary>
		/// <returns></returns>
		public ICollection<Type> ListServiceClasses()
		{
			return CollectionUtils.Map<ExtensionInfo, Type>(_serviceExtensionPoint.ListExtensions(),
				delegate(ExtensionInfo info) { return info.ExtensionClass; });
		}

		/// <summary>
		/// Lists the service contracts supported by this factory.
		/// </summary>
		/// <returns></returns>
		public ICollection<Type> ListServiceContracts()
		{
			return CollectionUtils.Map<ExtensionInfo, Type>(_serviceExtensionPoint.ListExtensions(),
				delegate(ExtensionInfo info)
				{
					ServiceImplementsContractAttribute a = CollectionUtils.FirstElement<ServiceImplementsContractAttribute>(
						info.ExtensionClass.GetCustomAttributes(typeof(ServiceImplementsContractAttribute), false));

					return a == null ? null : a.ServiceContract;
				});
		}

		/// <summary>
		/// Tests if this factory supports a service with the specified contract.
		/// </summary>
		/// <param name="serviceContract"></param>
		/// <returns></returns>
		public bool HasService(Type serviceContract)
		{
			return _serviceExtensionPoint.ListExtensions(new TypeExtensionFilter(serviceContract)).Length > 0;
		}
	}
}
