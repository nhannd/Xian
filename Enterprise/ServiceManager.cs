using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using Spring.Aop;
using Spring.Aop.Framework;
using Spring.Aop.Support;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Defines the extension point for all service layer implementations
    /// </summary>
    [ExtensionPoint()]
    public class ServiceLayerExtensionPoint : ExtensionPoint<IServiceLayer>
    {
    }

    /// <summary>
    /// Default implementation of <see cref="IServiceManager"/>, which allows an application to obtain
    /// a service that implements a specified interface.
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        private Dictionary<Type, ProxyFactory> _proxyFactoryCache;

        internal ServiceManager()
        {
            _proxyFactoryCache = new Dictionary<Type, ProxyFactory>();
        }

        public TServiceContract GetService<TServiceContract>()
        {
            return (TServiceContract)GetService(typeof(TServiceContract));
        }

        public IServiceLayer GetService(Type serviceContract)
        {
            if (!_proxyFactoryCache.ContainsKey(serviceContract))
            {
                CreateProxyFactory(serviceContract);
            }

            ProxyFactory factory = _proxyFactoryCache[serviceContract];
            return (IServiceLayer)factory.GetProxy();
        }

        public ICollection<Type> ListServices()
        {
            return CollectionUtils.Map<ExtensionInfo, Type>(new ServiceLayerExtensionPoint().ListExtensions(),
                delegate(ExtensionInfo info) { return info.ExtensionClass; });
        }


        /// <summary>
        /// Helper method to construct a Spring.NET proxy factory for the specified service
        /// </summary>
        /// <param name="serviceContract"></param>
        private void CreateProxyFactory(Type serviceContract)
        {
            ServiceLayerExtensionPoint xp = new ServiceLayerExtensionPoint();
            object service = xp.CreateExtension(new TypeExtensionFilter(serviceContract));

            // TODO this should all be optimized - we probably don't need to be creating all of these Spring.NET
            // objects from scratch each time, but need to look into that
            // in any case I don't think they are very expensive to create
            IAdvisor auditAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(ServiceOperationAttribute), true), new AuditAdvice());
            IAdvisor readContextAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(ReadOperationAttribute), true), new ReadContextAdvice());
            IAdvisor updateContextAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(UpdateOperationAttribute), true), new UpdateContextAdvice());
            IAdvisor transactionMonitorAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(UpdateOperationAttribute), true), new TransactionMonitorAdvice());

            ProxyFactory factory = new ProxyFactory(service);

            // transaction monitor advice should occur outside of audit advice
            factory.AddAdvisor(transactionMonitorAdvisor);

            // must add audit advice before context advice
            factory.AddAdvisor(auditAdvisor);

            // order of read/update context advice does not matter, because they are mutually exclusive
            // (only one or the other will ever be invoked)
            factory.AddAdvisor(readContextAdvisor);
            factory.AddAdvisor(updateContextAdvisor);

            _proxyFactoryCache.Add(serviceContract, factory);
        }
    }
}
