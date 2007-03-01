using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using Spring.Aop;
using Spring.Aop.Framework;
using Spring.Aop.Support;
using ClearCanvas.Common.Utilities;

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

        public ServiceFactory(IExtensionPoint serviceExtensionPoint)
        {
            _serviceExtensionPoint = serviceExtensionPoint;
            _proxyFactoryCache = new Dictionary<Type, ProxyFactory>();
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

        public ICollection<Type> ListServices()
        {
            return CollectionUtils.Map<ExtensionInfo, Type>(_serviceExtensionPoint.ListExtensions(),
                delegate(ExtensionInfo info) { return info.ExtensionClass; });
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
            object service = _serviceExtensionPoint.CreateExtension(new TypeExtensionFilter(serviceContract));

            ProxyFactory factory = new ProxyFactory(service);

            IAdvisor auditAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(ServiceOperationAttribute), true), new AuditAdvice());
            IAdvisor readContextAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(ReadOperationAttribute), true), new ReadContextAdvice());
            IAdvisor updateContextAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(UpdateOperationAttribute), true), new UpdateContextAdvice());
//            IAdvisor transactionMonitorAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(UpdateOperationAttribute), true), new TransactionMonitorAdvice());

            // order of read/update context advice does not matter, because they are mutually exclusive
            // (only one or the other will ever be invoked)
            factory.AddAdvisor(readContextAdvisor);
            factory.AddAdvisor(updateContextAdvisor);

            // must add audit advice inside of context advice, because it requires a persistence context to work
            factory.AddAdvisor(auditAdvisor);

            _proxyFactoryCache.Add(serviceContract, factory);
        }
    }
}
