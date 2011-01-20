#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Time;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Provides a consistent time to the application from the enterprise datasource specified in the nHibernate configuration
    /// 
    /// The initial call to CurrentTime will query the database to calculate an offset from the local machine time.  If the database query fails, the local time
    /// is returned, and subsequent calls to CurrentTime will attempt to re-query the database.  If the database query is successful, subsequent calls CurrentTime will 
    /// calculate the appropriate enterprise time from the local machine time using this offset.  The offset will be periodically re-calculated from 
    /// the database (by default, every 60 seconds).
    /// 
    /// If we are unable to syncronize the time for 10 minutes, a warning will be logged every 60 seconds when attempting to resynchronize.   
    /// </summary>    
    [ExtensionOf(typeof(TimeProviderExtensionPoint))]
    public class EnterpriseTimeProvider : ITimeProvider
    {
        private TimeSpan _localToEnterpriseOffset;
        private DateTime _lastResyncInLocalTime;
        private DateTime _lastSyncTime;
        private TimeSpan _resyncThreshold;
        private readonly TimeSpan _maxTimeBetweenSync;
        
        public EnterpriseTimeProvider()
        {
            _lastResyncInLocalTime = DateTime.MinValue;
            _resyncThreshold = new TimeSpan(0, 0, 60);
            _maxTimeBetweenSync = new TimeSpan(0, 10, 0);
        }

        #region ITimeProvider Members

        public DateTime CurrentTime
        {
            get 
            {
                if (ResyncRequired())
                {
                    ReSyncLocalToEnterpriseTime();
                }
                return EnterpriseTimeFromLocal(DateTime.Now); 
            }
        }

        public TimeSpan ResyncThreshold
        {
            get { return _resyncThreshold; }
            set { _resyncThreshold = value; }
        }

        #endregion

        private DateTime EnterpriseTimeFromLocal(DateTime localTime)
        {
            return localTime - _localToEnterpriseOffset;
        }

        private bool ResyncRequired()
        {
            if ( DateTime.Now - _lastResyncInLocalTime > _resyncThreshold)
            {
                return true;
            }
            return false;
        }

        private void ReSyncLocalToEnterpriseTime()
        {
            try
            {
                DateTime eTime = CurrentEnterpriseTime();
                _lastSyncTime = DateTime.Now;
                _localToEnterpriseOffset = _lastSyncTime - eTime;
            }
            catch (Exception)
            {
                if ((DateTime.Now - _lastSyncTime) > _maxTimeBetweenSync)
                    Platform.Log(LogLevel.Warn, "Unable to contact time server for synchronization");
            }
        }

        private DateTime CurrentEnterpriseTime()
        {
            DateTime time = default(DateTime);

            Platform.GetService(
				delegate(ITimeService service)
                {
                    time = service.GetTime(new GetTimeRequest()).Time;
                });

            _lastResyncInLocalTime = DateTime.Now;

            return time;
        }
    }
}
