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
		public CoreServiceProvider()
			:base(GetSettings())
		{

		}

		protected override string UserName
		{
			get { return LoginSession.Current.UserName; }
		}

		protected override string Password
		{
			get { return LoginSession.Current.SessionToken; }
		}

		private static RemoteServiceProviderArgs GetSettings()
		{
			return new RemoteServiceProviderArgs(
				WebServicesSettings.Default.ApplicationServicesBaseUrl,
				WebServicesSettings.Default.ConfigurationClass,
                WebServicesSettings.Default.MaxReceivedMessageSize,
                WebServicesSettings.Default.CertificateValidationMode,
                WebServicesSettings.Default.RevocationMode
				);
		}
    }
}
