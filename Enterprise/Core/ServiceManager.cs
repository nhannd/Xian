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

        public bool HasService(Type serviceContract)
        {
            return (new ServiceLayerExtensionPoint()).ListExtensions(new TypeExtensionFilter(serviceContract)).Length > 0;
        }


        /// <summary>
        /// Helper method to construct a Spring.NET proxy factory for the specified service
        /// </summary>
        /// <param name="serviceContract"></param>
        private void CreateProxyFactory(Type serviceContract)
        {
            ServiceLayerExtensionPoint xp = new ServiceLayerExtensionPoint();
            object service = xp.CreateExtension(new TypeExtensionFilter(serviceContract));

            ProxyFactory factory = new ProxyFactory(service);

            // TODO add advisors

            _proxyFactoryCache.Add(serviceContract, factory);
        }
    }
}
