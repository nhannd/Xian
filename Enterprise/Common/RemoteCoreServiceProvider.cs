#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
	[ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = false)]
	public partial class RemoteCoreServiceProvider : RemoteServiceProviderBase<EnterpriseCoreServiceAttribute>
	{
		public RemoteCoreServiceProvider()
			: base(GetSettings())
		{
		}

		private static RemoteServiceProviderArgs GetSettings()
		{
			return new RemoteServiceProviderArgs(
				RemoteCoreServiceSettings.Default.BaseUrl,
				RemoteCoreServiceSettings.Default.FailoverBaseUrl,
				RemoteCoreServiceSettings.Default.ConfigurationClass,
				RemoteCoreServiceSettings.Default.MaxReceivedMessageSize,
				RemoteCoreServiceSettings.Default.CertificateValidationMode,
				RemoteCoreServiceSettings.Default.RevocationMode,
				RemoteCoreServiceSettings.Default.UserCredentialsProviderClass
				)
					{
						FailedEndpointBlackoutTime = RemoteCoreServiceSettings.Default.FailedEndpointBlackoutTime
					};
		}
	}
}
