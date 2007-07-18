using System;
using System.Reflection;
using System.ServiceModel;

using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension of the <see cref="ServiceProviderExtensionPoint"/> that allows the client to obtain RIS application
    /// services.
    /// </summary>
    [ExtensionOf(typeof(ClearCanvas.Common.ServiceProviderExtensionPoint))]
    public class RemoteServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            try
            {
                if (LoginSession.Current == null)
                    throw new InvalidOperationException("User login credentials have not been provided");

				Uri uri = new Uri(new Uri("http://localhost:8000"), serviceType.FullName);
                EndpointAddress endpoint = new EndpointAddress(uri);
                WSHttpBinding binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.Message;
                binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                binding.MaxReceivedMessageSize = 1048576;
                //binding.ReceiveTimeout = new TimeSpan(0, 0 , 20);
                //binding.SendTimeout = new TimeSpan(0, 0, 10);

                // create the channel factory
                Type channelFactoryClass = typeof(ChannelFactory<>).MakeGenericType(new Type[] { serviceType });
                ChannelFactory channelFactory = (ChannelFactory)Activator.CreateInstance(channelFactoryClass, binding, endpoint);
                channelFactory.Credentials.UserName.UserName = LoginSession.Current.UserName;
                channelFactory.Credentials.UserName.Password = LoginSession.Current.Password;
                channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;

                // reflection is unfortunately the only way to create the service channel
                MethodInfo createChannelMethod = channelFactoryClass.GetMethod("CreateChannel", Type.EmptyTypes);
                object serviceProxy = createChannelMethod.Invoke(channelFactory, null);

                return serviceProxy;

            }
            catch (Exception e)
            {
                // in keeping with semantics of IServiceProvider, must return null here in order to give other
                // service providers the chance to provide the requested service
                // therefore, we just log the exception and don't rethrow it
                // (this is somewhat unfortunate, maybe we should not use the .NET IServiceProvider interface???)
                Platform.Log(e);

                return null;
            }
        }

        #endregion
    }
}
