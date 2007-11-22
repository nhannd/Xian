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
using Spring.Aop;
using Spring.Aop.Framework;
using Spring.Aop.Support;
using ClearCanvas.Common.Utilities;
using AopAlliance.Intercept;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Default implementation of <see cref="IServiceFactory"/>.  This implementation is thread-safe.
    /// </summary>
    public class ServiceFactory : IServiceFactory
    {
        private Dictionary<Type, ProxyFactory> _proxyFactoryCache;
        private IExtensionPoint _serviceExtensionPoint;
        private object _syncLock = new object();
        private event EventHandler<ServiceCreationEventArgs> _serviceCreation;

        public ServiceFactory(IExtensionPoint serviceExtensionPoint)
        {
            _serviceExtensionPoint = serviceExtensionPoint;
            _proxyFactoryCache = new Dictionary<Type, ProxyFactory>();
        }

        public event EventHandler<ServiceCreationEventArgs> ServiceCreation
        {
            add { _serviceCreation += value; }
            remove { _serviceCreation -= value; }
        }

        public TServiceContract GetService<TServiceContract>()
        {
            return (TServiceContract)GetService(typeof(TServiceContract));
        }

        public object GetService(Type serviceContract)
        {
            lock (_syncLock)
            {
                if (!_proxyFactoryCache.ContainsKey(serviceContract))
                {
                    CreateProxyFactory(serviceContract);
                }

                ProxyFactory factory = _proxyFactoryCache[serviceContract];
                object service = _serviceExtensionPoint.CreateExtension(new TypeExtensionFilter(serviceContract));
                factory.Target = service;
                return factory.GetProxy();
            }
        }

        public ICollection<Type> ListServiceClasses()
        {
            return CollectionUtils.Map<ExtensionInfo, Type>(_serviceExtensionPoint.ListExtensions(),
                delegate(ExtensionInfo info) { return info.ExtensionClass; });
        }

        public ICollection<Type> ListServiceContracts()
        {
            return CollectionUtils.Map<ExtensionInfo, Type>(_serviceExtensionPoint.ListExtensions(),
                delegate(ExtensionInfo info)
                {
                    ServiceImplementsContractAttribute a = CollectionUtils.FirstElement<ServiceImplementsContractAttribute>(
                        info.ExtensionClass.GetCustomAttributes(typeof(ServiceImplementsContractAttribute), false));

                    //TODO: should throw if a == null

                    return a == null ? null : a.ServiceContract;
                });
        }

        public bool HasService(Type serviceContract)
        {
            return _serviceExtensionPoint.ListExtensions(new TypeExtensionFilter(serviceContract)).Length > 0;
        }


        /// <summary>
        /// Helper method to construct a Spring.NET proxy factory for the specified service
        /// </summary>
        /// <param name="serviceContract"></param>
        private void CreateProxyFactory(Type serviceContract)
        {
            List<IMethodInterceptor> interceptors = new List<IMethodInterceptor>();
            interceptors.Add(new PersistenceContextAdvice());
            // must add audit advice inside of context advice, because it requires a persistence context to work
            interceptors.Add(new AuditAdvice());

            // allow addition of other interceptors by consumer
            EventsHelper.Fire(_serviceCreation, this, new ServiceCreationEventArgs(interceptors));

            // build proxy factory
            object service = _serviceExtensionPoint.CreateExtension(new TypeExtensionFilter(serviceContract));
            ProxyFactory factory = new ProxyFactory(service);
            foreach (IMethodInterceptor intercept in interceptors)
            {
                factory.AddAdvisor(new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(ServiceOperationAttribute), true), intercept));
            }

            _proxyFactoryCache.Add(serviceContract, factory);
        }
    }
}
