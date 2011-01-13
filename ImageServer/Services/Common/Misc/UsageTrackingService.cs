#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.UsageTracking;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.ImageServer.Services.Common.Misc
{
    /// <summary>
    /// Service to send usage information periodically
    /// </summary>
    internal class UsageTrackingService
    {
        #region Private Fields

        private Timer _timer;
        private readonly TimeSpan _repeatEvery24Hours = TimeSpan.FromHours(24);
        private DateTime _startTimestamp;
        private TimeSpan? _uptime;

        #endregion

        #region Public Methods

        public void Start()
        {
            _startTimestamp = DateTime.Now; // don't need to use Platform.Time

            _timer = new Timer(OnTimer, null, _repeatEvery24Hours, _repeatEvery24Hours);

            OnStartup();
        }

        public void Stop()
        {
            if (_timer!=null)
            {
                _timer.Dispose();
                _timer = null;
            }

            OnShutdown();
        }

        #endregion

        #region Private Methods

        private void OnStartup()
        {
            try
            {
                UsageMessage theMessage = CreateUsageMessage(UsageType.Startup);

                UsageUtilities.Register(theMessage, UsageTrackingThread.Background);
            }
            catch (Exception ex)
            {
                //catch everything or the shred host will crash
                Platform.Log(LogLevel.Error, ex, "Error occurred when trying to send usage tracking data");
            }
        }

        private UsageMessage CreateUsageMessage(UsageType type)
        {
            UsageMessage theMessage = UsageUtilities.GetUsageMessage();
            theMessage.MessageType = type;
            theMessage.Certified = ManifestVerification.Valid;

            AppendUsageData(theMessage);
            return theMessage;
        }

        private void OnShutdown()
        {
            try
            {
                _uptime = DateTime.Now - _startTimestamp;

                UsageMessage theMessage = CreateUsageMessage(UsageType.Shutdown);

                UsageUtilities.Register(theMessage, UsageTrackingThread.Current /* send in current thread */);
            }
            catch (Exception ex)
            {
                //catch everything or the shred host will crash
                Platform.Log(LogLevel.Error, ex, "Error occurred when trying to send usage tracking data");
            }
        }

        private void OnTimer(object nothing)
        {
            try
            {
                UsageMessage theMessage = CreateUsageMessage(UsageType.Other);

                AppendUsageData(theMessage);

                UsageUtilities.Register(theMessage, UsageTrackingThread.Background);
            }
            catch(Exception ex)
            {
                //catch everything or the shred host will crash
                Platform.Log(LogLevel.Error, ex, "Error occurred when trying to send usage tracking data");
            }
        }

        private void AppendUsageData(UsageMessage message)
        {
            message.AppData = new System.Collections.Generic.List<UsageApplicationData>();

            if (_uptime!=null)
            {
                message.AppData.Add(new UsageApplicationData()
                {
                    Key = "UPTIME",
                    Value = _uptime.Value.TotalHours.ToString(CultureInfo.InvariantCulture.NumberFormat),
                });
            }

            ImageServerData data = new ImageServerData();
            message.AppData.Add(new UsageApplicationData() {Key = "TOTAL_STUDIES",
                Value = data.TotalStudiesCount.ToString(CultureInfo.InvariantCulture.NumberFormat),
            });

            message.AppData.Add(new UsageApplicationData()
            {
                Key = "ACTIVE_DEVICES",
                Value = data.ActiveDeviceCount.ToString(CultureInfo.InvariantCulture.NumberFormat),
            });
        }

        #endregion
    }

    class ImageServerData
    {
        public int TotalStudiesCount { get; private set; }
        public int ActiveDeviceCount { get; private set; }

        public ImageServerData()
        {
            using(IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                TotalStudiesCount = 0;

                IServerPartitionEntityBroker partitionBroker = context.GetBroker<IServerPartitionEntityBroker>();
                var partitions = partitionBroker.Find(new ServerPartitionSelectCriteria());
                foreach(ServerPartition partition in partitions)
                {
                    TotalStudiesCount += partition.StudyCount;
                }

                IDeviceEntityBroker deviceBroker = context.GetBroker<IDeviceEntityBroker>();
                DeviceSelectCriteria criteria = new DeviceSelectCriteria();
                criteria.Enabled.EqualTo(true);
                ActiveDeviceCount += deviceBroker.Count(criteria);
            }
        }
    }
}
