using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    public abstract class RemoteServiceProviderBase<TRemoteServiceAttribute> : IServiceProvider
        where TRemoteServiceAttribute : Attribute
    {
        protected abstract string ServicesBaseUrl { get; }
        protected abstract ChannelFactory ConfigureChannelFactory(Type channelFactoryClass, Uri serviceUri, bool authenticationRequired);

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            // check if the service is provided by this provider
            if (!AttributeUtils.HasAttribute<TRemoteServiceAttribute>(serviceType))
                return null;

            AuthenticationAttribute authAttr = AttributeUtils.GetAttribute<AuthenticationAttribute>(serviceType);
            bool authenticationRequired = authAttr == null ? true : authAttr.AuthenticationRequired;

            // create the channel
            object channel = CreateChannel(serviceType, authenticationRequired);

            Platform.Log(LogLevel.Debug, "Created WCF channel instance for service {0}, authenticationRequired={1}.",
                         serviceType.FullName, authenticationRequired);

            return channel;
        }

        #endregion

        private object CreateChannel(Type serviceType, bool authenticationRequired)
        {
            Uri uri = new Uri(new Uri(ServicesBaseUrl), serviceType.FullName);
            Type channelFactoryClass = typeof(ChannelFactory<>).MakeGenericType(new Type[] { serviceType });


            ChannelFactory channelFactory = ConfigureChannelFactory(
                channelFactoryClass,
                uri,
                authenticationRequired);

            // invoke the CreateChannel method on the factory
            MethodInfo createChannelMethod = channelFactory.GetType().GetMethod("CreateChannel", Type.EmptyTypes);
            return createChannelMethod.Invoke(channelFactory, null);
        }
    }
}
