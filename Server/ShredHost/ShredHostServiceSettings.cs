#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Server.ShredHost
{
	internal sealed partial class ShredHostServiceSettings
	{
		public const int DefaultShredHostHttpPort = 51121;
		public const int DefaultSharedHttpPort = 51122;
		public const int DefaultSharedTcpPort = 51123;
		public const string DefaultServiceAddressBase = "";

		private static readonly Proxy _instance = new Proxy();

		public static Proxy Instance
		{
			get { return _instance; }
		}

		public sealed class Proxy
		{
			internal Proxy() {}

			private object this[string propertyName]
			{
				get { return Default[propertyName]; }
				set { ApplicationSettingsExtensions.SetSharedPropertyValue(Default, propertyName, value); }
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
				Default.Save();
			}
		}
	}
}