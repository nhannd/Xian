using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming
{
    internal class HeaderStreamingStatistics : StatisticsSet
    {
        #region Constructors

        public HeaderStreamingStatistics()
            : base("HeaderStreaming")
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