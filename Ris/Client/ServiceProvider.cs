using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Reflection;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Common
{
    [ExtensionOf(typeof(ClearCanvas.Common.ServiceProviderExtensionPoint))]
    public class ServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            string url = string.Format("http://localhost:8000/{0}", serviceType.FullName);
            EndpointAddress endpoint = new EndpointAddress(url);
            BasicHttpBinding binding = new BasicHttpBinding();

            Type channelFactoryClass = typeof(ChannelFactory<>).MakeGenericType(new Type[] { serviceType });
            object channelFactory = Activator.CreateInstance(channelFactoryClass, binding, endpoint);

            // reflection is unfortunately the only way to create the service channel
            MethodInfo createChannelMethod = channelFactoryClass.GetMethod("CreateChannel", Type.EmptyTypes);
            object serviceProxy = createChannelMethod.Invoke(channelFactory, null);

            return serviceProxy;
        }

        #endregion
    }
}
