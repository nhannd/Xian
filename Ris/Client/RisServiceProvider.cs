#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
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
