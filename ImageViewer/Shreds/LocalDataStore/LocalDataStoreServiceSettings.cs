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

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[SettingsGroupDescription("Configuration for the local data store service.")]
	[SettingsProvider(typeof (LocalFileSettingsProvider))]
	internal sealed partial class LocalDataStoreServiceSettings
	{
		public const string DefaultStorageDirectory = @"c:\dicom_datastore\filestore\";
		public const string DefaultBadFileDirectory = @"c:\dicom_datastore\badfiles\";
		public const uint DefaultSendReceiveImportConcurrency = 2;
		public const uint DefaultDatabaseUpdateFrequencyMilliseconds = 5000;
		public const uint DefaultPurgeTimeMinutes = 120;

		private static Proxy _instance;

		public static Proxy Instance
		{
			get { return _instance ?? (_instance = new Proxy(Default)); }
		}

		public sealed class Proxy
		{
			private readonly LocalDataStoreServiceSettings _settings;

			public Proxy(LocalDataStoreServiceSettings settings)
			{
				_settings = settings;
			}

			private object this[string propertyName]
			{
				get { return _settings[propertyName]; }
				set { ApplicationSettingsExtensions.SetSharedPropertyValue(_settings, propertyName, value); }
			}

			[DefaultValue(DefaultStorageDirectory)]
			public string StorageDirectory
			{
				get { return (string) this["StorageDirectory"]; }
				set { this["StorageDirectory"] = value; }
			}

			[DefaultValue(DefaultBadFileDirectory)]
			public string BadFileDirectory
			{
				get { return (string) this["BadFileDirectory"]; }
				set { this["BadFileDirectory"] = value; }
			}

			[DefaultValue(DefaultSendReceiveImportConcurrency)]
			public uint SendReceiveImportConcurrency
			{
				get { return (uint) this["SendReceiveImportConcurrency"]; }
				set { this["SendReceiveImportConcurrency"] = value; }
			}

			[DefaultValue(DefaultDatabaseUpdateFrequencyMilliseconds)]
			public uint DatabaseUpdateFrequencyMilliseconds
			{
				get { return (uint) this["DatabaseUpdateFrequencyMilliseconds"]; }
				set { this["DatabaseUpdateFrequencyMilliseconds"] = value; }
			}

			[DefaultValue(DefaultPurgeTimeMinutes)]
			public uint PurgeTimeMinutes
			{
				get { return (uint) this["PurgeTimeMinutes"]; }
				set { this["PurgeTimeMinutes"] = value; }
			}

			public void Save()
			{
				_settings.Save();
			}
		}
	}
}