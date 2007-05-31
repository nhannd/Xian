using System;
using System.Collections.Generic;
using System.Text;
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

		#endregion

		public override object Clone()
		{
			LocalDataStoreServiceSettings clone = new LocalDataStoreServiceSettings();

			clone.StorageDirectory = _instance.StorageDirectory;
			clone.BadFileDirectory = _instance.BadFileDirectory;
			clone.SendReceiveImportConcurrency = _instance.SendReceiveImportConcurrency;
			clone.DatabaseUpdateFrequencyMilliseconds = _instance.DatabaseUpdateFrequencyMilliseconds;

			return clone;
		}
	}
}
