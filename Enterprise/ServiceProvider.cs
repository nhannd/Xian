using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise
{
    [ExtensionOf(typeof(ClearCanvas.Common.ServiceProviderExtensionPoint))]
    public class ServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceContract)
        {
            try
            {
                return Core.ServiceManager.GetService(serviceContract);
            }
            catch (NotSupportedException)
            {
                // according to MSDN, IServiceProvider contract is to return null if service not supported
                return null;
            }
        }

        #endregion
    }
}
