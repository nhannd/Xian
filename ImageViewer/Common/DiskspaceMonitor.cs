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
using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
    /// <summary>
    /// Helper class to determine disk usage
    /// </summary>
    public static class LocalStorageMonitor
    {
        #region Private Fields

        static Dictionary<string, Diskspace> _cached = new Dictionary<string, Diskspace>();
        static private object _sync = new object();
        static DateTime? _scheduledRefreshTime;

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if max used space is exceeded in the local study storage location
        static public bool IsMaxUsedSpaceExceeded
        {
            get
            {
                lock (_sync)
                {
                    RefreshDiskspace();

                    return FileStoreDiskSpace.UsedSpacePercent > StorageConfiguration.MaximumUsedSpacePercent;
                }
            }
            
        }


        static DateTime? _lastConfigUpdate;
        static  string _fileStorePath;
        static StorageConfiguration _storageConfig;
        static Diskspace _diskspace;

        public static double MaximumUsedSpacePercent
        {
            get 
            {
                return StorageConfiguration.MaximumUsedSpacePercent;
            }
        }

        public static double UsedSpacePercent
        {
            get
            {
                return FileStoreDiskSpace.UsedSpacePercent;
            }
        }


        private static StorageConfiguration StorageConfiguration
        {
            get
            {
                if (!_lastConfigUpdate.HasValue || _lastConfigUpdate.Value.OlderThan(TimeSpan.FromSeconds(10)))
                {

                    double prevMaxUsed = _storageConfig != null ? _storageConfig.MaximumUsedSpacePercent : 0;
                    _storageConfig = StudyStore.GetConfiguration();

                    // detect change
                    if (prevMaxUsed == 0 || prevMaxUsed != _storageConfig.MaximumUsedSpacePercent)
                    {
                        FileStoreDiskSpace = null;
                        RefreshDiskspace();
                    }
                    _lastConfigUpdate = Platform.Time;
                }

                return _storageConfig;
            }
        }


        private static Diskspace FileStoreDiskSpace 
        {
            get
            {
                if (_diskspace == null)
                {
                    _diskspace = new Diskspace(FileStoreRootPath);
                }

                return _diskspace;
            }
            set { _diskspace = value; }
        }

        private static string FileStoreRootPath
        {
            get
            {
                return StorageConfiguration.FileStoreRootPath;
            }
        }

        #endregion

        #region Private Methods


        static private void RefreshDiskspace()
        {
            var needRefresh = !_scheduledRefreshTime.HasValue || FileStoreDiskSpace.LastRefresh < _scheduledRefreshTime;
            
            if (needRefresh)
            {
                FileStoreDiskSpace.Refresh();

                const long GB = 1024 * 1024 * 1024;
                double delay;
                double remain = StorageConfiguration.MaximumUsedSpaceBytes - FileStoreDiskSpace.UsedSpace;

                // Note: Ideally we should calculate the how fast the usage is changing and estimate how long it will take to reach the max level
                if (Math.Abs(remain) <= 5 * GB)
                    // within the critical level window. Check more often.
                    delay = 15; // 15 seconds
                else
                    delay = 5 * 60; // 5 minutes.

                _scheduledRefreshTime = Platform.Time.AddSeconds(delay);


                if (remain>0 && remain < StorageMonitorSettings.Default.LowStorageWarningThresholdInMB * 1024 * 1024)
                {
                    Platform.Log(LogLevel.Warn, "File Storage usage is approaching the critical limit!!");
                }

                Platform.Log(LogLevel.Debug, "Diskspace updated. Check again in {0} seconds", delay);
            }

        }

        #endregion
    }

    static public class DateTimeExtensions
    {
        public static bool OlderThan(this DateTime dt, TimeSpan span)
        {
            return (Platform.Time - dt) > span;
        }
    }
}
