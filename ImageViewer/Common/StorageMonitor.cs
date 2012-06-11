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
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
    public static class StorageSpaceMonitor
    {
        const long Unknown = -1;

        private static DateTime? _configValidUtil;
        private static long? _lastKnownSpace = Unknown;
        private static StorageConfiguration _storageConfig;
        private static object _syncLock = new object();

        private static long _totalUsed = Unknown;
        private static long _maxUsed = Unknown;

        public static long TotalUsed
        {
            get
            {
                EnsureUpToDate();
                return _totalUsed;
            }
            private set { _totalUsed = value; }
        }

        public static long MaxUsed
        {
            get
            {
                EnsureUpToDate();
                return _maxUsed;
            }
            private set { _maxUsed = value; }
        }

        public static long GetAvailableStorageSpace()
        {
            EnsureUpToDate();
            return _lastKnownSpace.HasValue ? _lastKnownSpace.Value : 0;
        }


        private static void EnsureUpToDate()
        {
            try
            {
                lock (_syncLock)
                {
                    if (!_configValidUtil.HasValue || (DateTime.Now > _configValidUtil))
                    {
                        _storageConfig = StudyStore.GetConfiguration();
                        var validDuration = StorageMonitorSettings.Default.MaxDiskChecksPerHour > 0 ? 3600 / StorageMonitorSettings.Default.MaxDiskChecksPerHour : 60;
                        _configValidUtil = DateTime.Now.Add(TimeSpan.FromSeconds(validDuration));

                        TotalUsed = _storageConfig.FileStoreDiskSpace.UsedSpace / 1024 / 1024;
                        MaxUsed = _storageConfig.MaximumUsedSpaceBytes / 1024 / 1024;
                        _lastKnownSpace = Math.Max(MaxUsed - TotalUsed, 0);

                        if (_lastKnownSpace > 0 && _lastKnownSpace <= StorageMonitorSettings.Default.LowStorageWarningThresholdInMB)
                        {
                            Platform.Log(LogLevel.Warn, "Storage space is approaching the critical limit");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex, "Unable to determine disk space");
            }

        }

    }
}
