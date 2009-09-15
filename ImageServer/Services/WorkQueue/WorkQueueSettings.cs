#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.ImageServer.Services.WorkQueue {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
	internal sealed class WorkQueueSettings : ShredConfigSection
	{
		public const int DefaultWorkQueueQueryDelay = 10000;
		public const int DefaultWorkQueueThreadCount = 10;
		public const int DefaultPriorityWorkQueueThreadCount = 2;
		public const int DefaultMemoryLimitedWorkQueueThreadCount = 4;
		public const int DefaultWorkQueueMinimumFreeMemoryMB = 256;
	    public const bool DefaultEnableStudyIntegrityValidation = true;
        public const int DefaultTierMigrationProgressUpdateInSeconds = 30;

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

		/// <summary>
		/// Enable/disable study integrity validation during work queue processing
		/// </summary>
		[SettingsDescriptionAttribute("The number of seconds delay between attempting to process a queue entry.")]
        [ConfigurationProperty("EnableStudyIntegrityValidation", DefaultValue = DefaultEnableStudyIntegrityValidation)]
        public bool EnableStudyIntegrityValidation
		{
            get { return ((bool)(this["EnableStudyIntegrityValidation"])); }
            set { this["EnableStudyIntegrityValidation"] = value; }
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
		
		[ConfigurationProperty("WorkQueueMinimumFreeMemoryMB", DefaultValue = DefaultWorkQueueMinimumFreeMemoryMB)]
		public int WorkQueueMinimumFreeMemoryMB
		{
			get { return ((int)(this["WorkQueueMinimumFreeMemoryMB"])); }
			set { this["WorkQueueMinimumFreeMemoryMB"] = value; }
		}

        /// <summary>
        /// The number of seconds to update on the progress of tier migration work queue entries.
        /// </summary>
        [SettingsDescriptionAttribute("The number of seconds to update on the progress of tier migration work queue entries.")]
        [ConfigurationProperty("TierMigrationProgressUpdateInSeconds", DefaultValue = DefaultTierMigrationProgressUpdateInSeconds)]
        public int TierMigrationProgressUpdateInSeconds
        {
            get { return ((int)(this["TierMigrationProgressUpdateInSeconds"])); }
            set { this["TierMigrationProgressUpdateInSeconds"] = value; }
        }




		#endregion

		public override object Clone()
		{
			WorkQueueSettings clone = new WorkQueueSettings();

			clone.WorkQueueQueryDelay = _instance.WorkQueueQueryDelay;
			clone.WorkQueueThreadCount = _instance.WorkQueueThreadCount;
			clone.PriorityWorkQueueThreadCount = _instance.PriorityWorkQueueThreadCount;
			clone.MemoryLimitedWorkQueueThreadCount = _instance.MemoryLimitedWorkQueueThreadCount;
			clone.WorkQueueMinimumFreeMemoryMB = _instance.WorkQueueMinimumFreeMemoryMB;
		    clone.EnableStudyIntegrityValidation = _instance.EnableStudyIntegrityValidation;
		    clone.TierMigrationProgressUpdateInSeconds = _instance.TierMigrationProgressUpdateInSeconds;
			return clone;
		}
	}
}
