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

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed partial class LocalDataStoreServiceSettings
	{
		public const string DefaultStorageDirectory = @"c:\dicom_datastore\filestore\";
		public const string DefaultBadFileDirectory = @"c:\dicom_datastore\badfiles\";
		public const uint DefaultSendReceiveImportConcurrency = 2;
		public const uint DefaultDatabaseUpdateFrequencyMilliseconds = 5000;
		public const uint DefaultPurgeTimeMinutes = 120;

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
				Default.Save();
			}
		}
	}
}