using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Common
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
                Platform.Log(LogLevel.Info, "Accessing {0} using {1}", serviceType, this.GetType());
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
