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

    public class ServiceManager
    {
        private Session _session;

        public ServiceManager(Session session)
        {
            _session = session;
        }

        public TServiceInterface GetService<TServiceInterface>()
        {
            // TODO this should all be optimized - we probably don't need to be creating all of these Spring.NET
            // objects from scratch each time, but need to look into that

            ServiceLayerExtensionPoint xp = new ServiceLayerExtensionPoint();
            object service = xp.CreateExtension(new TypeExtensionFilter(typeof(TServiceInterface)));
            
            IAdvisor auditAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(ServiceOperationAttribute), true), new AuditAdvice(_session));
            IAdvisor readContextAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(ReadOperationAttribute), true), new ReadContextAdvice(_session));
            IAdvisor updateContextAdvisor = new DefaultPointcutAdvisor(new AttributeMatchMethodPointcut(typeof(UpdateOperationAttribute), true), new UpdateContextAdvice(_session));

            ProxyFactory factory = new ProxyFactory(service);
            // must add audit advice before context advice
            factory.AddAdvisor(auditAdvisor);

            // order of read/update context advice does not matter, because they are mutually exclusive
            factory.AddAdvisor(readContextAdvisor);
            factory.AddAdvisor(updateContextAdvisor);

            IServiceLayer serviceLayerProxy = (IServiceLayer)factory.GetProxy();
            serviceLayerProxy.SetSession(_session);
            return (TServiceInterface)serviceLayerProxy;
        }
    }
}
