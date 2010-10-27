#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClearCanvas.Server.ShredHost
{
	public class ShredHostServiceSettings : ShredConfigSection
	{
		public const int DefaultShredHostHttpPort = 51121;
		public const int DefaultSharedHttpPort = 51122;
		public const int DefaultSharedTcpPort = 50123;

		private static ShredHostServiceSettings _instance;

		private ShredHostServiceSettings()
		{
		}

		public static string SettingName
		{
			get { return "ShredHostServiceSettings"; }
		}

		public static ShredHostServiceSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = ShredConfigManager.GetConfigSection(ShredHostServiceSettings.SettingName) as ShredHostServiceSettings;
					if (_instance == null)
					{
						_instance = new ShredHostServiceSettings();
						ShredConfigManager.UpdateConfigSection(ShredHostServiceSettings.SettingName, _instance);
					}
				}

				return _instance;
			}
		}

		public static void Save()
		{
			ShredConfigManager.UpdateConfigSection(ShredHostServiceSettings.SettingName, _instance);
		}

		#region Public Properties

		[ConfigurationProperty("ShredHostHttpPort", DefaultValue = ShredHostServiceSettings.DefaultShredHostHttpPort)]
		public int ShredHostHttpPort
		{
			get { return (int)this["ShredHostHttpPort"]; }
			set { this["ShredHostHttpPort"] = value; }
		}

		[ConfigurationProperty("SharedHttpPort", DefaultValue = ShredHostServiceSettings.DefaultSharedHttpPort)]
		public int SharedHttpPort
		{
			get { return (int)this["SharedHttpPort"]; }
			set { this["SharedHttpPort"] = value; }
		}

		[ConfigurationProperty("SharedTcpPort", DefaultValue = ShredHostServiceSettings.DefaultSharedTcpPort)]
		public int SharedTcpPort
		{
			get { return (int)this["SharedTcpPort"]; }
			set { this["SharedTcpPort"] = value; }
		}

		#endregion

		public override object Clone()
		{
			ShredHostServiceSettings clone = new ShredHostServiceSettings();

			clone.ShredHostHttpPort = _instance.ShredHostHttpPort;
			clone.SharedHttpPort = _instance.SharedHttpPort;
			clone.SharedTcpPort = _instance.SharedTcpPort;

			return clone;
		}
	}
}
