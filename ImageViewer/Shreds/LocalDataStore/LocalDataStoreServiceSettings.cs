#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
