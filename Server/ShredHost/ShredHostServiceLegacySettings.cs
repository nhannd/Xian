#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;

namespace ClearCanvas.Server.ShredHost
{
	[Obsolete("Use ShredHostServiceSettings")]
	[LegacyShredConfigSection("ShredHostServiceSettings")]
	internal sealed class LegacyShredHostServiceSettings : ShredConfigSection, IMigrateLegacyShredConfigSection
	{
		public const int DefaultShredHostHttpPort = 51121;
		public const int DefaultSharedHttpPort = 51122;
		public const int DefaultSharedTcpPort = 50123;
		public const string DefaultServiceAddressBase = "";

		private static LegacyShredHostServiceSettings _instance;

		private LegacyShredHostServiceSettings()
		{
		}

		public static string SettingName
		{
			get { return "ShredHostServiceSettings"; }
		}

		public static LegacyShredHostServiceSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = ShredConfigManager.GetConfigSection(LegacyShredHostServiceSettings.SettingName) as LegacyShredHostServiceSettings;
					if (_instance == null)
					{
						_instance = new LegacyShredHostServiceSettings();
						ShredConfigManager.UpdateConfigSection(LegacyShredHostServiceSettings.SettingName, _instance);
					}
				}

				return _instance;
			}
		}

		public static void Save()
		{
			ShredConfigManager.UpdateConfigSection(LegacyShredHostServiceSettings.SettingName, _instance);
		}

		#region Public Properties

		[ConfigurationProperty("ShredHostHttpPort", DefaultValue = LegacyShredHostServiceSettings.DefaultShredHostHttpPort)]
		public int ShredHostHttpPort
		{
			get { return (int)this["ShredHostHttpPort"]; }
			set { this["ShredHostHttpPort"] = value; }
		}

		[ConfigurationProperty("SharedHttpPort", DefaultValue = LegacyShredHostServiceSettings.DefaultSharedHttpPort)]
		public int SharedHttpPort
		{
			get { return (int)this["SharedHttpPort"]; }
			set { this["SharedHttpPort"] = value; }
		}

		[ConfigurationProperty("SharedTcpPort", DefaultValue = LegacyShredHostServiceSettings.DefaultSharedTcpPort)]
		public int SharedTcpPort
		{
			get { return (int)this["SharedTcpPort"]; }
			set { this["SharedTcpPort"] = value; }
		}

		[ConfigurationProperty("ServiceAddressBase", DefaultValue = LegacyShredHostServiceSettings.DefaultServiceAddressBase)]
		public string ServiceAddressBase
		{
			get { return (string)this["ServiceAddressBase"]; }
			set { this["ServiceAddressBase"] = value; ; }
		}

		#endregion

		public override object Clone()
		{
			LegacyShredHostServiceSettings clone = new LegacyShredHostServiceSettings();

			clone.ShredHostHttpPort = _instance.ShredHostHttpPort;
			clone.SharedHttpPort = _instance.SharedHttpPort;
			clone.SharedTcpPort = _instance.SharedTcpPort;
			clone.ServiceAddressBase = _instance.ServiceAddressBase;

			return clone;
		}

		void IMigrateLegacyShredConfigSection.Migrate()
		{
			ShredHostServiceSettings.Instance.ShredHostHttpPort = ShredHostHttpPort;
			ShredHostServiceSettings.Instance.SharedHttpPort = SharedHttpPort;
			ShredHostServiceSettings.Instance.SharedTcpPort = SharedTcpPort;
			ShredHostServiceSettings.Instance.ServiceAddressBase = ServiceAddressBase;
			ShredHostServiceSettings.Instance.Save();
		}
	}
}
