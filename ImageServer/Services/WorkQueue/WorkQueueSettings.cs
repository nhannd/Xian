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

using System;
using System.Configuration;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Services.WorkQueue {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
	internal sealed class WorkQueueSettings : ShredConfigSection
	{
		public const int DefaultWorkQueueQueryDelay = 10000;
		public const int DefaultWorkQueueMaxFailureCount = 3;
		public const int DefaultWorkQueueFailureDelayMinutes = 3;
		public const int DefaultWorkQueueProcessDelayMedPrioritySeconds = 15;
		public const int DefaultWorkQueueExpireDelaySeconds = 90;
		public const int DefaultWorkQueueProcessDelayLowPrioritySeconds = 45;
		public const int DefaultWorkQueueProcessDelayHighPrioritySeconds = 1;
		public const int DefaultLowPriorityMaxBatchSize = 100;
		public const int DefaultMedPriorityMaxBatchSize = 250;
		public const int DefaultWorkQueueThreadCount = 10;
		public const int DefaultPriorityWorkQueueThreadCount = 2;
		public const int DefaultMemoryLimitedWorkQueueThreadCount = 4;
		public const string DefaultNonMemoryLimitedWorkQueueTypes = "DeleteStudy,WebDeleteStudy,MigrateStudy,PurgeStudy";

		private static WorkQueueSettings _instance;

		private WorkQueueSettings()
		{
		}

		public static string SettingName
		{
			get { return "WorkQueueSettings"; }
		}

		public static WorkQueueSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = ShredConfigManager.GetConfigSection(SettingName) as WorkQueueSettings;
					if (_instance == null)
					{
						_instance = new WorkQueueSettings();
						ShredConfigManager.UpdateConfigSection(SettingName, _instance);
					}
				}

				return _instance;
			}
		}

		public static void Save()
		{
			ShredConfigManager.UpdateConfigSection(SettingName, _instance);
		}

		#region Public Properties

		[ConfigurationProperty("WorkQueueQueryDelay", DefaultValue = DefaultWorkQueueQueryDelay)]
		public int WorkQueueQueryDelay
		{
			get { return ((int)(this["WorkQueueQueryDelay"])); }
			set { this["WorkQueueQueryDelay"] = value; }
		}

		[ConfigurationProperty("WorkQueueMaxFailureCount", DefaultValue = DefaultWorkQueueMaxFailureCount)]
		public int WorkQueueMaxFailureCount
		{
			get { return ((int)(this["WorkQueueMaxFailureCount"])); }
			set { this["WorkQueueMaxFailureCount"] = value; }
		}

		[ConfigurationProperty("WorkQueueFailureDelayMinutes", DefaultValue = DefaultWorkQueueFailureDelayMinutes)]
		public int WorkQueueFailureDelayMinutes
		{
			get { return ((int)(this["WorkQueueFailureDelayMinutes"])); }
			set { this["WorkQueueFailureDelayMinutes"] = value; }
		}

		/// <summary>
		/// The number of seconds delay between attempting to process a queue entry.
		/// </summary>
		[SettingsDescriptionAttribute("The number of seconds delay between attempting to process a queue entry.")]
		[ConfigurationProperty("WorkQueueProcessDelayMedPrioritySeconds", DefaultValue = DefaultWorkQueueProcessDelayMedPrioritySeconds)]
		public int WorkQueueProcessDelayMedPrioritySeconds
		{
			get { return ((int)(this["WorkQueueProcessDelayMedPrioritySeconds"])); }
			set { this["WorkQueueProcessDelayMedPrioritySeconds"] = value; }
		}

		/// <summary>
		/// The number of seconds to delay after processing until the queue entry is deleted.
		/// </summary>
		[SettingsDescriptionAttribute("The number of seconds to delay after processing until the queue entry is deleted.")]
		[ConfigurationProperty("WorkQueueExpireDelaySeconds", DefaultValue = DefaultWorkQueueExpireDelaySeconds)]
		public int WorkQueueExpireDelaySeconds
		{
			get { return ((int)(this["WorkQueueExpireDelaySeconds"])); }
			set { this["WorkQueueExpireDelaySeconds"] = value; }
		}

		[ConfigurationProperty("WorkQueueProcessDelayLowPrioritySeconds", DefaultValue = DefaultWorkQueueProcessDelayLowPrioritySeconds)]
		public int WorkQueueProcessDelayLowPrioritySeconds
		{
			get { return ((int)(this["WorkQueueProcessDelayLowPrioritySeconds"])); }
			set { this["WorkQueueProcessDelayLowPrioritySeconds"] = value; }
		}

		[ConfigurationProperty("WorkQueueProcessDelayHighPrioritySeconds", DefaultValue = DefaultWorkQueueProcessDelayHighPrioritySeconds)]
		public int WorkQueueProcessDelayHighPrioritySeconds
		{
			get { return ((int)(this["WorkQueueProcessDelayHighPrioritySeconds"])); }
			set { this["WorkQueueProcessDelayHighPrioritySeconds"] = value; }
		}

		[ConfigurationProperty("LowPriorityMaxBatchSize", DefaultValue = DefaultLowPriorityMaxBatchSize)]
		public int LowPriorityMaxBatchSize
		{
			get { return ((int)(this["LowPriorityMaxBatchSize"])); }
			set { this["LowPriorityMaxBatchSize"] = value; }
		}

		[ConfigurationProperty("MedPriorityMaxBatchSize", DefaultValue = DefaultMedPriorityMaxBatchSize)]
		public int MedPriorityMaxBatchSize
		{
			get { return ((int)(this["MedPriorityMaxBatchSize"])); }
			set { this["MedPriorityMaxBatchSize"] = value; }
		}

		[ConfigurationProperty("WorkQueueThreadCount", DefaultValue = DefaultWorkQueueThreadCount)]
		public int WorkQueueThreadCount
		{
			get { return ((int)(this["WorkQueueThreadCount"])); }
			set { this["WorkQueueThreadCount"] = value; }
		}

		[ConfigurationProperty("PriorityWorkQueueThreadCount", DefaultValue = DefaultPriorityWorkQueueThreadCount)]
		public int PriorityWorkQueueThreadCount
		{
			get { return ((int)(this["PriorityWorkQueueThreadCount"])); }
			set { this["PriorityWorkQueueThreadCount"] = value; }
		}
		[ConfigurationProperty("MemoryLimitedWorkQueueThreadCount", DefaultValue = DefaultMemoryLimitedWorkQueueThreadCount)]
		public int MemoryLimitedWorkQueueThreadCount
		{
			get { return ((int)(this["MemoryLimitedWorkQueueThreadCount"])); }
			set { this["MemoryLimitedWorkQueueThreadCount"] = value; }
		}

		[ConfigurationProperty("NonMemoryLimitedWorkQueueTypes", DefaultValue = DefaultNonMemoryLimitedWorkQueueTypes)]
		public string NonMemoryLimitedWorkQueueTypes
		{
			get { return ((string)(this["NonMemoryLimitedWorkQueueTypes"])); }
			set { this["NonMemoryLimitedWorkQueueTypes"] = value; }
		}
		#endregion

		public override object Clone()
		{
			WorkQueueSettings clone = new WorkQueueSettings();

			clone.WorkQueueQueryDelay = _instance.WorkQueueQueryDelay;
			clone.WorkQueueMaxFailureCount = _instance.WorkQueueMaxFailureCount;
			clone.WorkQueueFailureDelayMinutes = _instance.WorkQueueFailureDelayMinutes;
			clone.WorkQueueProcessDelayMedPrioritySeconds = _instance.WorkQueueProcessDelayMedPrioritySeconds;
			clone.WorkQueueExpireDelaySeconds = _instance.WorkQueueExpireDelaySeconds;
			clone.WorkQueueProcessDelayLowPrioritySeconds = _instance.WorkQueueProcessDelayLowPrioritySeconds;
			clone.WorkQueueProcessDelayHighPrioritySeconds = _instance.WorkQueueProcessDelayHighPrioritySeconds;
			clone.LowPriorityMaxBatchSize = _instance.LowPriorityMaxBatchSize;
			clone.MedPriorityMaxBatchSize = _instance.MedPriorityMaxBatchSize;
			clone.WorkQueueThreadCount = _instance.WorkQueueThreadCount;
			clone.PriorityWorkQueueThreadCount = _instance.PriorityWorkQueueThreadCount;
			clone.MemoryLimitedWorkQueueThreadCount = _instance.MemoryLimitedWorkQueueThreadCount;
			clone.NonMemoryLimitedWorkQueueTypes = _instance.NonMemoryLimitedWorkQueueTypes;
			return clone;
		}
	}
}
