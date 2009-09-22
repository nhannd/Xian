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
using System.Collections.Generic;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

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
				return _proxyGenerator.CreateInterfaceProxyWithTarget(serviceContract, service, _interceptors.ToArray());
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
