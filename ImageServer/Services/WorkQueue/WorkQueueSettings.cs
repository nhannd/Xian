#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    internal partial class WorkQueueSettings
    {
        public const int DefaultWorkQueueQueryDelay = 10000;
        public const int DefaultWorkQueueThreadCount = 10;
        public const int DefaultPriorityWorkQueueThreadCount = 2;
        public const int DefaultMemoryLimitedWorkQueueThreadCount = 4;
        public const int DefaultWorkQueueMinimumFreeMemoryMB = 256;
        public const bool DefaultEnableStudyIntegrityValidation = true;
        public const int DefaultTierMigrationProgressUpdateInSeconds = 30;


        private static WorkQueueSettingsProxy _instance;

        public static WorkQueueSettingsProxy Instance
        {
            get { return _instance ?? (_instance = new WorkQueueSettingsProxy(Default)); }
        }

        public sealed class WorkQueueSettingsProxy
        {
            private readonly WorkQueueSettings _settings;

            public WorkQueueSettingsProxy(WorkQueueSettings settings)
            {
                _settings = settings;
            }

            private object this[string propertyName]
            {
                get { return _settings[propertyName]; }
                set { ApplicationSettingsExtensions.SetSharedPropertyValue(_settings, propertyName, value); }
            }

            [DefaultValue(DefaultWorkQueueQueryDelay)]
            public int WorkQueueQueryDelay
            {
                get { return (int)this["WorkQueueQueryDelay"]; }
                set { this["WorkQueueQueryDelay"] = value; }
            }

            [DefaultValue(DefaultEnableStudyIntegrityValidation)]
            public bool EnableStudyIntegrityValidation
            {
                get { return (bool)this["EnableStudyIntegrityValidation"]; }
                set { this["EnableStudyIntegrityValidation"] = value; }
            }

            [DefaultValue(DefaultMemoryLimitedWorkQueueThreadCount)]
            public int MemoryLimitedWorkQueueThreadCount
            {
                get { return (int)this["MemoryLimitedWorkQueueThreadCount"]; }
                set { this["MemoryLimitedWorkQueueThreadCount"] = value; }
            }

            [DefaultValue(DefaultPriorityWorkQueueThreadCount)]
            public int PriorityWorkQueueThreadCount
            {
                get { return (int)this["PriorityWorkQueueThreadCount"]; }
                set { this["PriorityWorkQueueThreadCount"] = value; }
            }

            [DefaultValue(DefaultTierMigrationProgressUpdateInSeconds)]
            public int TierMigrationProgressUpdateInSeconds
            {
                get { return (int)this["TierMigrationProgressUpdateInSeconds"]; }
                set { this["TierMigrationProgressUpdateInSeconds"] = value; }
            }

            [DefaultValue(DefaultWorkQueueMinimumFreeMemoryMB)]
            public int WorkQueueMinimumFreeMemoryMB
            {
                get { return (int)this["WorkQueueMinimumFreeMemoryMB"]; }
                set { this["WorkQueueMinimumFreeMemoryMB"] = value; }
            }

            [DefaultValue(DefaultWorkQueueThreadCount)]
            public int WorkQueueThreadCount
            {
                get { return (int)this["WorkQueueThreadCount"]; }
                set { this["WorkQueueThreadCount"] = value; }
            }


            public void Save()
            {
                _settings.Save();
            }
        }
    }
}
