using System;

namespace ClearCanvas.ImageServer.Web.Application.SeriesDetails
{
    /// <summary>
    /// Model object behind the <see cref="SeriesDetailsPanel"/>
    /// </summary>
    public class SeriesDetails
    {
        private string _seriesInstanceUid;
        private string _modality;
        private string _seriesNumber;
        private string _seriesDescription;
        private int _numberOfSeriesRelatedInstances;
        private DateTime? _performedDateTime;
        private string _sourceApplicationEntityTitle;

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        public string SeriesNumber
        {
            get { return _seriesNumber; }
            set { _seriesNumber = value; }
        }

        public string SeriesDescription
        {
            get { return _seriesDescription; }
            set { _seriesDescription = value; }
        }

        public int NumberOfSeriesRelatedInstances
        {
            get { return _numberOfSeriesRelatedInstances; }
            set { _numberOfSeriesRelatedInstances = value; }
        }

        public DateTime? PerformedDateTime
        {
            get { return _performedDateTime; }
            set { _performedDateTime = value; }
        }

        public string SourceApplicationEntityTitle
        {
            get { return _sourceApplicationEntityTitle; }
            set { _sourceApplicationEntityTitle = value; }
        }
    }
}
