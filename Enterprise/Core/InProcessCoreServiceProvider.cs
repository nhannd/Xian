using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// This service provider allows the application server to make use of core services internally
    /// by providing these services in-process.
    /// </summary>
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    public class InProcessCoreServiceProvider : IServiceProvider
    {
        private IServiceFactory _serviceFactory;

        public InProcessCoreServiceProvider()
        {
            _serviceFactory = new ServiceFactory(new CoreServiceExtensionPoint());
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (_serviceFactory.HasService(serviceType))
            {
                return _serviceFactory.GetService(serviceType);
            }
            else
            {
                return null;    // as per MSDN
            }
        }

        #endregion
    }
}
