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
        public delegate object CreateChannelDelegate();

        private static readonly Dictionary<Type, CreateChannelDelegate> _channelFactoryMethods = new Dictionary<Type, CreateChannelDelegate>();
        protected const int OneMegaByte = 1048576;

        protected abstract string ApplicationServicesBaseUrl { get; }
        protected abstract bool AuthenticationRequired(Type serviceType);
        protected abstract void ValidateAuthentification();
        protected abstract void GetValidationInfo(out string userName, out string password);
        protected abstract Binding GetBinding();

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            try
            {
                bool servicesNotImplementedByThisProvider = AttributeUtils.GetAttribute<TRemoteServiceAttribute>(serviceType) == null;
                if (servicesNotImplementedByThisProvider)
                    return null;

                bool authenticationRequired = this.AuthenticationRequired(serviceType);
                if (authenticationRequired)
                    ValidateAuthentification();

                // obtain the channel factory method for this service
                CreateChannelDelegate factoryMethod;
                if(!_channelFactoryMethods.TryGetValue(serviceType, out factoryMethod))
                {
                    lock (_channelFactoryMethods)
                    {
                        // don't need double-checked lock pattern here because who cares if multiple threads create the same channel
                        _channelFactoryMethods.Add(serviceType,
                                               factoryMethod = CreateChannelFactory(serviceType, authenticationRequired));
                    }
                }

                // call the factory method to create the channel
                object channel = factoryMethod();

                Platform.Log(LogLevel.Debug, "Created WCF channel instance for service {0}, authenticated={1}.",
                             serviceType.FullName, authenticationRequired);

                return channel;
            }
            catch (Exception e)
            {
                // in keeping with semantics of IServiceProvider, must return null here in order to give other
                // service providers the chance to provide the requested service
                // therefore, we just log the exception and don't rethrow it
                // (this is somewhat unfortunate, maybe we should not use the .NET IServiceProvider interface???)
                Platform.Log(LogLevel.Error, e);

                return null;
            }
        }

        #endregion

        private CreateChannelDelegate CreateChannelFactory(Type serviceType, bool authenticationRequired)
        {
            string baseUrl = this.ApplicationServicesBaseUrl;

            Uri uri = new Uri(new Uri(baseUrl), serviceType.FullName);
            EndpointAddress endpoint = new EndpointAddress(uri);
            Binding binding = this.GetBinding();

            Type channelFactoryClass = typeof(ChannelFactory<>).MakeGenericType(new Type[] { serviceType });
            ChannelFactory channelFactory = (ChannelFactory)Activator.CreateInstance(channelFactoryClass, binding, endpoint);
            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;

            if (authenticationRequired)
            {
                string userName, password;
                GetValidationInfo(out userName, out password);
                channelFactory.Credentials.UserName.UserName = userName;
                channelFactory.Credentials.UserName.Password = password;
            }

            // create a delegate and bind it to the channelFactory instance
            MethodInfo createChannelMethod = channelFactory.GetType().GetMethod("CreateChannel", Type.EmptyTypes);
            CreateChannelDelegate createChannel = (CreateChannelDelegate)
                Delegate.CreateDelegate(typeof(CreateChannelDelegate), channelFactory, createChannelMethod);
            return createChannel;
        }
    }
}
