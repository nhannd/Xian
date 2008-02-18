using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderRetrieval
{
    internal class HeaderRetrievalStatistics:StatisticsSet
    {
        #region Constructors
        public HeaderRetrievalStatistics()
            :base("HeaderRetreival")
        {

            AddField(new Statistics<string>("Client"));
            AddField(new Statistics<string>("StudyInstanceUid"));
            AddField(new TimeSpanStatistics("ProcessTime"));
        }

        #endregion Constructors
        
        #region Public Properties

        public TimeSpanStatistics ProcessTime
        {
            get { return this["ProcessTime"] as TimeSpanStatistics; }
        }

        #endregion Public Properties
    }
}
