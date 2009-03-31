using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Common
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = false)]
	public class RemoteCoreServiceProvider : RemoteServiceProviderBase<EnterpriseCoreServiceAttribute>
	{
		public RemoteCoreServiceProvider()
			: base(GetSettings())
		{
		}

		private static RemoteServiceProviderArgs GetSettings()
		{
			return new RemoteServiceProviderArgs(
				RemoteCoreServiceSettings.Default.BaseUrl,
				RemoteCoreServiceSettings.Default.ConfigurationClass,
				RemoteCoreServiceSettings.Default.MaxReceivedMessageSize,
				RemoteCoreServiceSettings.Default.CertificateValidationMode,
				RemoteCoreServiceSettings.Default.RevocationMode
				);
		}
	}
}
