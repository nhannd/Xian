using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal class LocalDataStoreServiceSettings : ShredConfigSection
	{
		public const string DefaultStorageFolder = @"c:\dicom_datastore\filestore\";
		public const string DefaultBadFileFolder = @"c:\dicom_datastore\badfiles\";
		public const int DefaultSendReceiveImportConcurrency = 2;

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

		[ConfigurationProperty("StorageFolder", DefaultValue = LocalDataStoreServiceSettings.DefaultStorageFolder)]
		public string StorageFolder
		{
			get { return (string)this["StorageFolder"]; }
			set { this["StorageFolder"] = value; }
		}

		[ConfigurationProperty("BadFileFolder", DefaultValue = LocalDataStoreServiceSettings.DefaultBadFileFolder)]
		public string BadFileFolder
		{
			get { return (string)this["BadFileFolder"]; }
			set { this["BadFileFolder"] = value; }
		}

		[ConfigurationProperty("SendReceiveImportConcurrency", DefaultValue = LocalDataStoreServiceSettings.DefaultSendReceiveImportConcurrency)]
		public int SendReceiveImportConcurrency
		{
			get { return (int)this["SendReceiveImportConcurrency"]; }
			set { this["SendReceiveImportConcurrency"] = value; }
		}

		#endregion

		public override object Clone()
		{
			LocalDataStoreServiceSettings clone = new LocalDataStoreServiceSettings();

			clone.StorageFolder = _instance.StorageFolder;
			clone.BadFileFolder = _instance.BadFileFolder;
			clone.SendReceiveImportConcurrency = _instance.SendReceiveImportConcurrency;

			return clone;
		}
	}
}
