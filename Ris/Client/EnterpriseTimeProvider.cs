using System;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client
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
    class EnterpriseTimeProvider : ITimeProvider
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
            DateTime time;
            ITimeService service;

            try
            {
                service = ApplicationContext.GetService<ITimeService>();
                time = service.GetTime();

                _lastResyncInLocalTime = DateTime.Now;
            }
            // Null reference exception can be thrown during Platform.StartApp().  This is because there is no current Session defined,
            // but nHibernate will attempt to instantiate an instance of all objects referenced in the nHibernate mapping files, some of which 
            // may initialise members with Platform.Time
            catch (NullReferenceException e)
            { 
                Platform.Log("Cannot instantiate EnterpriseTimeProvider, defaulting to LocalTimeProvider");
                Platform.Log(e);

                time = DateTime.Now;
            }

            return time;
        }
    }
}
