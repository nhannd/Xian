using System;

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Provides a consistent time to the application from the enterprise datasource specified in the nHibernate configuration
    /// 
    /// The initial call to CurrentTime will query the database to calculate an offset from the local machine time.  If the database query fails, the local time
    /// is returned, and subsequent calls to CurrentTime will attempt to re-query the database.  If the database query is successful, subsequent calls CurrentTime will 
    /// calculate the appropriate enterprise time from the local machine time using this offset.  The offset will be periodically re-calculated from 
    /// the database (by default, every 60 seconds).  
    /// </summary>    
    [ExtensionOf(typeof(TimeProviderExtensionPoint))]
    public class EnterpriseTimeProvider : ITimeProvider
    {
        private TimeSpan _localToEnterpriseOffset;
        private DateTime _lastResyncInLocalTime;
        private TimeSpan _resyncThreshold;
        
        public EnterpriseTimeProvider()
        {
            _lastResyncInLocalTime = DateTime.MinValue;
            _resyncThreshold = new TimeSpan(0, 0, 60);
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
            _localToEnterpriseOffset = DateTime.Now - CurrentEnterpriseTime();
        }

        private DateTime CurrentEnterpriseTime()
        {
            DateTime time = default(DateTime);

            Platform.GetService<ITimeService>(
                delegate(ITimeService service)
                {
                    time = service.GetTime();
                });

            _lastResyncInLocalTime = DateTime.Now;

            return time;
        }
    }
}
