#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
                    time = service.GetTime(new GetTimeRequest()).Time;
                });

            _lastResyncInLocalTime = DateTime.Now;

            return time;
        }
    }
}
