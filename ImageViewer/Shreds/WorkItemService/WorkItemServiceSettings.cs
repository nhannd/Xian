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

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
	internal sealed class WorkItemServiceSettings : ShredConfigSection
	{
		public const int DefaultNormalThreadCount = 6;
		public const int DefaultStatThreadCount = 2;
        public const uint DefaultPostponeSeconds = 15;
        public const int DefaultExpireDelaySeconds = 60;
        public const uint DefaultDeleteDelayMinutes = 120;
        public const int DefaultRetryCount = 3;
        public const int DefaultStudyProcessBatchSize = 25;
		private static WorkItemServiceSettings _instance;

        private WorkItemServiceSettings()
		{
		}

		public static string SettingName
		{
            get { return "WorkItemServiceSettings"; }
		}

        public static WorkItemServiceSettings Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = ShredConfigManager.GetConfigSection(SettingName) as WorkItemServiceSettings;
					if (_instance == null)
					{
                        _instance = new WorkItemServiceSettings();
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

        [ConfigurationProperty("NormalThreadCount", DefaultValue = DefaultNormalThreadCount)]
        public int NormalThreadCount
		{
			get { return (int)this["NormalThreadCount"]; }
            set { this["NormalThreadCount"] = value; }
		}

        [ConfigurationProperty("StatThreadCount", DefaultValue = DefaultStatThreadCount)]
        public int StatThreadCount
		{
            get { return (int)this["StatThreadCount"]; }
            set { this["StatThreadCount"] = value; }
		}

        [ConfigurationProperty("PostponeSeconds", DefaultValue = DefaultPostponeSeconds)]
        public uint PostponeSeconds
		{
            get { return (uint)this["PostponeSeconds"]; }
            set { this["PostponeSeconds"] = value; }
		}

        [ConfigurationProperty("ExpireDelaySeconds", DefaultValue = DefaultExpireDelaySeconds)]
        public int ExpireDelaySeconds
        {
            get { return (int)this["ExpireDelaySeconds"]; }
            set { this["ExpireDelaySeconds"] = value; }
        }

        [ConfigurationProperty("DeleteDelayMinutes", DefaultValue = DefaultDeleteDelayMinutes)]
        public uint DeleteDelayMinutes
        {
            get { return (uint)this["DeleteDelayMinutes"]; }
            set { this["DeleteDelayMinutes"] = value; }
        }

        [ConfigurationProperty("RetryCount", DefaultValue = DefaultRetryCount)]
        public int RetryCount
        {
            get { return (int)this["RetryCount"]; }
            set { this["RetryCount"] = value; }
        }

        [ConfigurationProperty("StudyProcessBatchSize", DefaultValue = DefaultStudyProcessBatchSize)]
        public int StudyProcessBatchSize
        {
            get { return (int)this["StudyProcessBatchSize"]; }
            set { this["StudyProcessBatchSize"] = value; }
        }
        
		#endregion

		public override object Clone()
		{
		    var clone = new WorkItemServiceSettings
		                    {
                                StatThreadCount = _instance.StatThreadCount,
                                NormalThreadCount = _instance.NormalThreadCount,
		                        PostponeSeconds = _instance.PostponeSeconds,
                                ExpireDelaySeconds = _instance.ExpireDelaySeconds,
                                StudyProcessBatchSize = _instance.StudyProcessBatchSize,
                                RetryCount =  _instance.RetryCount
		                    };

		    return clone;
		}
	}
}
