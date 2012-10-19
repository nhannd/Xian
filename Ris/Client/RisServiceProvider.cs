#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension of the <see cref="ServiceProviderExtensionPoint"/> that allows the client to obtain RIS application
	/// services.
	/// </summary>
	[ExtensionOf(typeof(ServiceProviderExtensionPoint))]
	internal class RisServiceProvider : RemoteServiceProviderBase<RisApplicationServiceAttribute>
	{
		public RisServiceProvider()
			: base(GetSettings())
		{
		}

		private static RemoteServiceProviderArgs GetSettings()
		{
			return new RemoteServiceProviderArgs(
				WebServicesSettings.Default.ApplicationServicesBaseUrl,
				null,	// no failover defined
				WebServicesSettings.Default.ConfigurationClass,
				WebServicesSettings.Default.MaxReceivedMessageSize,
				WebServicesSettings.Default.CertificateValidationMode,
				WebServicesSettings.Default.RevocationMode
				);
		}
	}
}
