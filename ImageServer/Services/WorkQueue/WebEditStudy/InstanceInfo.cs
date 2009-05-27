namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    class InstanceInfo
    {
        private string _seriesInstanceUid;
        private string _sopInstanceUid;

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string SopInstanceUid
        {
            get { return _sopInstanceUid; }
            set { _sopInstanceUid = value; }
        }
    }
}