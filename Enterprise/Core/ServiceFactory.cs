#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using Castle.DynamicProxy;

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
        private event EventHandler<ServiceCreationEventArgs> _serviceCreation;

        /// <summary>
        /// Constructs a service factory that instantiates services based on the specified extension point.
        /// </summary>
        /// <param name="serviceExtensionPoint"></param>
        public ServiceFactory(IExtensionPoint serviceExtensionPoint)
        {
            _serviceExtensionPoint = serviceExtensionPoint;
            _proxyGenerator = new ProxyGenerator();
        }

        /// <summary>
        /// Occurs when a new service instance is created.
        /// </summary>
        public event EventHandler<ServiceCreationEventArgs> ServiceCreation
        {
            add { _serviceCreation += value; }
            remove { _serviceCreation -= value; }
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
                // create a list of interceptors
                List<IInterceptor> interceptors = new List<IInterceptor>();
                interceptors.Add(new PersistenceContextAdvice());
                // must add audit advice inside of context advice, because it requires a persistence context to work
                interceptors.Add(new AuditAdvice());

                // allow addition of other interceptors by consumer
                EventsHelper.Fire(_serviceCreation, this, new ServiceCreationEventArgs(interceptors));

                // instantiate service object
                object service = _serviceExtensionPoint.CreateExtension(new TypeExtensionFilter(serviceContract));

                // combine service and interceptors
                AopInterceptorChain aopIntercept = new AopInterceptorChain(interceptors);

                // note: _proxyGenerator does internal caching based on service contract
                // so subsequent calls based on the same contract will be fast
                return _proxyGenerator.CreateProxy(serviceContract, aopIntercept, service);
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
