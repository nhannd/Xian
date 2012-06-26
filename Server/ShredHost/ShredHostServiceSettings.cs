#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Server.ShredHost
{
	[SettingsGroupDescription("Configuration for the Shred Host Service.")]
	[SettingsProvider(typeof (LocalFileSettingsProvider))]
	internal sealed partial class ShredHostServiceSettings
	{
		public const int DefaultShredHostHttpPort = 51121;
		public const int DefaultSharedHttpPort = 51122;
		public const int DefaultSharedTcpPort = 51123;
		public const string DefaultServiceAddressBase = "";

		private static Proxy _instance;

		public static Proxy Instance
		{
			get { return _instance ?? (_instance = new Proxy(Default)); }
		}

		public sealed class Proxy
		{
			private readonly ShredHostServiceSettings _settings;

			public Proxy(ShredHostServiceSettings settings)
			{
				_settings = settings;
			}

			private object this[string propertyName]
			{
				get { return _settings[propertyName]; }
				set { ApplicationSettingsExtensions.SetSharedPropertyValue(_settings, propertyName, value); }
			}

			[DefaultValue(DefaultShredHostHttpPort)]
			public int ShredHostHttpPort
			{
				get { return (int) this["ShredHostHttpPort"]; }
				set { this["ShredHostHttpPort"] = value; }
			}

			[DefaultValue(DefaultSharedHttpPort)]
			public int SharedHttpPort
			{
				get { return (int) this["SharedHttpPort"]; }
				set { this["SharedHttpPort"] = value; }
			}

			[DefaultValue(DefaultSharedTcpPort)]
			public int SharedTcpPort
			{
				get { return (int) this["SharedTcpPort"]; }
				set { this["SharedTcpPort"] = value; }
			}

			[DefaultValue(DefaultServiceAddressBase)]
			public string ServiceAddressBase
			{
				get { return (string) this["ServiceAddressBase"]; }
				set { this["ServiceAddressBase"] = value; }
			}

			public void Save()
			{
				_settings.Save();
			}
		}
	}
}