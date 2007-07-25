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
