using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension of the <see cref="ServiceProviderExtensionPoint"/> that allows the client to obtain Core Enterprise services.
    /// </summary>
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal sealed class CoreServiceProvider : RemoteServiceProviderBase<EnterpriseCoreServiceAttribute>
    {
        private IServiceChannelConfiguration _channelConfiguration;

        private IServiceChannelConfiguration ChannelConfiguration
        {
            get
            {
                if (_channelConfiguration == null)
                {
                    Type configClass = Type.GetType(WebServicesSettings.Default.ConfigurationClass);
                    _channelConfiguration = (IServiceChannelConfiguration)Activator.CreateInstance(configClass);
                }
                return _channelConfiguration;
            }
        }

        protected override string ServicesBaseUrl
        {
            get { return WebServicesSettings.Default.ApplicationServicesBaseUrl; }
        }

        protected override ChannelFactory ConfigureChannelFactory(Type channelFactoryClass, Uri serviceUri, bool authenticationRequired)
        {
            ChannelFactory channelFactory = ChannelConfiguration.ConfigureChannelFactory(
                new ServiceChannelConfigurationArgs(channelFactoryClass, serviceUri, authenticationRequired, 
                    WebServicesSettings.Default.MaxReceivedMessageSize,
                    WebServicesSettings.Default.CertificateValidationMode,
                    WebServicesSettings.Default.RevocationMode));

            if (authenticationRequired)
            {
                channelFactory.Credentials.UserName.UserName = LoginSession.Current.UserName;
                channelFactory.Credentials.UserName.Password = LoginSession.Current.SessionToken;
            }
            return channelFactory;
        }
    }
}
