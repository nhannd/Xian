using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ClearCanvas.ImageServer.Core.Edit
{
    /// <summary>
    /// Enumerated values for different levels of deletion.
    /// </summary>
    public enum DeletionLevel
    {
        Study,
        Series
    }

    [XmlRoot("WebDeleteWorkQueueEntry")]
    public class WebDeleteWorkQueueEntryData
    {
        private DeletionLevel _level;
        private string _reason;

        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        public DeletionLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }
    }

    [XmlRoot("WebDeleteWorkQueueEntry")]
    public class WebDeleteStudyLevelQueueData : WebDeleteWorkQueueEntryData
    {

    }

    /// <summary>
    /// Encapsulate the object stored in the Data column of the "DeleteSeries" WorkQueue entry.
    /// </summary>
    [XmlRoot("WebDeleteWorkQueueEntry")]
    public class WebDeleteSeriesLevelQueueData : WebDeleteWorkQueueEntryData
    {
        private string _userName;
        private DateTime _timestamp;
        private List<string> _seriesInstanceUids;

        public WebDeleteSeriesLevelQueueData()
        {
            this.Level = DeletionLevel.Series;
        }

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

        public List<string> SeriesInstanceUids
        {
            get { return _seriesInstanceUids; }
            set { _seriesInstanceUids = value; }
        }

    }
}