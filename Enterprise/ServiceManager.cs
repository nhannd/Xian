using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using Spring.Aop;
using Spring.Aop.Framework;
using Spring.Aop.Support;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Defines the extension point for all service layer implementations
    /// </summary>
    [ExtensionPoint()]
    public class ServiceLayerExtensionPoint : ExtensionPoint<IServiceLayer>
    {
    }

    public class ServiceManager : IServiceManager
    {
        private Session _session;
        private Dictionary<Type, ProxyFactory> _proxyFactoryCache;

        public ServiceManager(Session session)
        {
            _session = session;
            _proxyFactoryCache = new Dictionary<Type, ProxyFactory>();
        }

        public TServiceInterface GetService<TServiceInterface>()
        {
            if (!_proxyFactoryCache.ContainsKey(typeof(TServiceInterface)))
            {
                CreateProxyFactory(typeof(TServiceInterface));
            }

            ProxyFactory factory = _proxyFactoryCache[typeof(TServiceInterface)];
            IServiceLayer serviceLayerProxy = (IServiceLayer)factory.GetProxy();
            serviceLayerProxy.SetSession(_session);
            return (TServiceInterface)serviceLayerProxy;
        }

        /// <summary>
        /// Helper method to construct a Spring.NET proxy factory for the specified service
        /// </summary>
        /// <param name="serviceInterfaceType"></param>
        private void CreateProxyFactory(Type serviceInterfaceType)
        {
            ServiceLayerExtensionPoint xp = new ServiceLayerExtensionPoint();
            object service = xp.CreateExtension(new TypeExtensionFilter(serviceInterfaceType));

            // TODO this should all be optimized - we probably don't need to be creating all of these Spring.NET
            // objects from scratch each time, but need to look into that
            // in any case I don't think they are very expensive to create
            IAdvisor auditAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(ServiceOperationAttribute), true), new AuditAdvice(_session));
            IAdvisor readContextAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(ReadOperationAttribute), true), new ReadContextAdvice(_session));
            IAdvisor updateContextAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(UpdateOperationAttribute), true), new UpdateContextAdvice(_session));
            IAdvisor transactionMonitorAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(UpdateOperationAttribute), true), new TransactionMonitorAdvice(_session));

            ProxyFactory factory = new ProxyFactory(service);

            // transaction monitor advice should occur outside of audit advice
            factory.AddAdvisor(transactionMonitorAdvisor);

            // must add audit advice before context advice
            factory.AddAdvisor(auditAdvisor);

            // order of read/update context advice does not matter, because they are mutually exclusive
            // (only one or the other will ever be invoked)
            factory.AddAdvisor(readContextAdvisor);
            factory.AddAdvisor(updateContextAdvisor);

            _proxyFactoryCache.Add(serviceInterfaceType, factory);
        }
    }
}
