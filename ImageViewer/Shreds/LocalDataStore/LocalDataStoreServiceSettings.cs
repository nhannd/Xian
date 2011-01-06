#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class LocalDataStoreServiceSettings : ShredConfigSection
	{
		public const string DefaultStorageDirectory = @"c:\dicom_datastore\filestore\";
		public const string DefaultBadFileDirectory = @"c:\dicom_datastore\badfiles\";
		public const uint DefaultSendReceiveImportConcurrency = 2;
		public const uint DefaultDatabaseUpdateFrequencyMilliseconds = 5000;
		public const uint DefaultPurgeTimeMinutes = 120;

		private static LocalDataStoreServiceSettings _instance;

		private LocalDataStoreServiceSettings()
		{
		}

		public static string SettingName
		{
			get { return "LocalDataStoreServiceSettings"; }
		}

		public static LocalDataStoreServiceSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = ShredConfigManager.GetConfigSection(LocalDataStoreServiceSettings.SettingName) as LocalDataStoreServiceSettings;
					if (_instance == null)
					{
						_instance = new LocalDataStoreServiceSettings();
						ShredConfigManager.UpdateConfigSection(LocalDataStoreServiceSettings.SettingName, _instance);
					}
				}

				return _instance;
			}
		}

		public static void Save()
		{
			ShredConfigManager.UpdateConfigSection(LocalDataStoreServiceSettings.SettingName, _instance);
		}

		#region Public Properties

		[ConfigurationProperty("StorageDirectory", DefaultValue = LocalDataStoreServiceSettings.DefaultStorageDirectory)]
		public string StorageDirectory
		{
			get { return (string)this["StorageDirectory"]; }
			set { this["StorageDirectory"] = value; }
		}

		[ConfigurationProperty("BadFileDirectory", DefaultValue = LocalDataStoreServiceSettings.DefaultBadFileDirectory)]
		public string BadFileDirectory
		{
			get { return (string)this["BadFileDirectory"]; }
			set { this["BadFileDirectory"] = value; }
		}

		[ConfigurationProperty("SendReceiveImportConcurrency", DefaultValue = LocalDataStoreServiceSettings.DefaultSendReceiveImportConcurrency)]
		public uint SendReceiveImportConcurrency
		{
			get { return (uint)this["SendReceiveImportConcurrency"]; }
			set { this["SendReceiveImportConcurrency"] = value; }
		}

		[ConfigurationProperty("DatabaseUpdateFrequencyMilliseconds", DefaultValue = LocalDataStoreServiceSettings.DefaultDatabaseUpdateFrequencyMilliseconds)]
		public uint DatabaseUpdateFrequencyMilliseconds
		{
			get { return (uint)this["DatabaseUpdateFrequencyMilliseconds"]; }
			set { this["DatabaseUpdateFrequencyMilliseconds"] = value; }
		}

		[ConfigurationProperty("PurgeTimeMinutes", DefaultValue = LocalDataStoreServiceSettings.DefaultPurgeTimeMinutes)]
		public uint PurgeTimeMinutes
		{
			get { return (uint)this["PurgeTimeMinutes"]; }
			set { this["PurgeTimeMinutes"] = value; }
		}

		#endregion

		public override object Clone()
		{
			LocalDataStoreServiceSettings clone = new LocalDataStoreServiceSettings();

			clone.StorageDirectory = _instance.StorageDirectory;
			clone.BadFileDirectory = _instance.BadFileDirectory;
			clone.SendReceiveImportConcurrency = _instance.SendReceiveImportConcurrency;
			clone.DatabaseUpdateFrequencyMilliseconds = _instance.DatabaseUpdateFrequencyMilliseconds;
			clone.PurgeTimeMinutes = _instance.PurgeTimeMinutes;

			return clone;
		}
	}
}
