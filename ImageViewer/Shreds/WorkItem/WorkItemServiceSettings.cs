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

namespace ClearCanvas.ImageViewer.Shreds.WorkItem
{
	internal sealed class WorkItemServiceSettings : ShredConfigSection
	{
		public const uint DefaultNormalThreadCount = 4;
		public const uint DefaultStatThreadCount = 4;
        public const uint DefaultPostponeSeconds = 30;
        public const uint DefaultExpireDelaySeconds = 90;
        public const uint DefaultRetryCount = 3;

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
                    _instance = ShredConfigManager.GetConfigSection(WorkItemServiceSettings.SettingName) as WorkItemServiceSettings;
					if (_instance == null)
					{
                        _instance = new WorkItemServiceSettings();
                        ShredConfigManager.UpdateConfigSection(WorkItemServiceSettings.SettingName, _instance);
					}
				}

				return _instance;
			}
		}

		public static void Save()
		{
            ShredConfigManager.UpdateConfigSection(WorkItemServiceSettings.SettingName, _instance);
		}

		#region Public Properties

        [ConfigurationProperty("NormalThreadCount", DefaultValue = WorkItemServiceSettings.DefaultNormalThreadCount)]
        public uint NormalThreadCount
		{
			get { return (uint)this["NormalThreadCount"]; }
            set { this["NormalThreadCount"] = value; }
		}

        [ConfigurationProperty("StatThreadCount", DefaultValue = WorkItemServiceSettings.DefaultStatThreadCount)]
        public uint StatThreadCount
		{
            get { return (uint)this["StatThreadCount"]; }
            set { this["StatThreadCount"] = value; }
		}

        [ConfigurationProperty("PostponeSeconds", DefaultValue = WorkItemServiceSettings.DefaultPostponeSeconds)]
        public uint PostponeSeconds
		{
            get { return (uint)this["PostponeSeconds"]; }
            set { this["PostponeSeconds"] = value; }
		}

        [ConfigurationProperty("ExpireDelaySeconds", DefaultValue = WorkItemServiceSettings.DefaultExpireDelaySeconds)]
        public uint ExpireDelaySeconds
        {
            get { return (uint)this["ExpireDelaySeconds"]; }
            set { this["ExpireDelaySeconds"] = value; }
        }

        [ConfigurationProperty("RetryCount", DefaultValue = WorkItemServiceSettings.DefaultRetryCount)]
        public uint RetryCount
        {
            get { return (uint)this["RetryCount"]; }
            set { this["RetryCount"] = value; }
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
		                    };

		    return clone;
		}
	}
}
