#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    ///  Marks an interface as Image Server service interface (as opposed to an Enterpise service).
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ImageServerServiceAttribute : Attribute { }

    /// <summary>
    /// Provide access to the Image Server services hosted on another machine.
    /// </summary>
    [ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = false)]
    public class RemoteImageServerServiceProvider : RemoteServiceProviderBase<ImageServerServiceAttribute>
    {
        public RemoteImageServerServiceProvider()
			: base(GetSettings())
		{
		}

		private static RemoteServiceProviderArgs GetSettings()
		{
			return new RemoteServiceProviderArgs(
                RemoteImageServerServiceSettings.Default.BaseUrl,
                RemoteImageServerServiceSettings.Default.FailoverBaseUrl,
                RemoteImageServerServiceSettings.Default.ConfigurationClass,
                RemoteImageServerServiceSettings.Default.MaxReceivedMessageSize,
                RemoteImageServerServiceSettings.Default.CertificateValidationMode,
                RemoteImageServerServiceSettings.Default.RevocationMode,
                RemoteImageServerServiceSettings.Default.UserCredentialsProviderClass
				);
		}
    }
}
