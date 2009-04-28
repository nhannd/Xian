using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    public class InProcessApplicationServiceProvider : IServiceProvider
    {
        private readonly IServiceFactory _serviceFactory;

        public InProcessApplicationServiceProvider()
        {
            _serviceFactory = new ServiceFactory(new ApplicationServiceExtensionPoint());
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