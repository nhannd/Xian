using System;

namespace ClearCanvas.ImageServer.Core.Edit
{
    /// <summary>
    /// Encapsulate the object stored in the Data column of the "DeleteSeries" WorkQueue entry.
    /// </summary>
    public class DeleteSeriesQueueData
    {
        private string _userName;
        private DateTime _timestamp;
        private string _seriesInstanceUid;
        private string _reason;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }
    }
}