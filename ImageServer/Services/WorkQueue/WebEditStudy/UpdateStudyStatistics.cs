using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Stores statistics of a WorkQueue instance processing.
    /// </summary>
    internal class UpdateStudyStatistics : StatisticsSet
    {
        #region Constructors

        public UpdateStudyStatistics(string studyInstanceUid)
            : this("UpdateStudy", studyInstanceUid)
        { }

        public UpdateStudyStatistics(string name, string studyInstanceUid)
            : base(name)
        {
            AddField("StudyInstanceUid", studyInstanceUid);
        }

        #endregion Constructors

        #region Public Properties

        
        public TimeSpanStatistics ProcessTime
        {
            get
            {
                if (this["ProcessTime"] == null)
                    this["ProcessTime"] = new TimeSpanStatistics("ProcessTime");

                return (this["ProcessTime"] as TimeSpanStatistics);
            }
            set { this["ProcessTime"] = value; }
        }

        public ulong StudySize
        {
            set
            {
                this["StudySize"] = new ByteCountStatistics("StudySize", value);
            }
            get
            {
                if (this["StudySize"] == null)
                    this["StudySize"] = new ByteCountStatistics("StudySize");

                return ((ByteCountStatistics)this["StudySize"]).Value;
            }
        }

        public int InstanceCount
        {
            set
            {
                this["InstanceCount"] = new Statistics<int>("InstanceCount", value);
            }
            get
            {
                if (this["InstanceCount"] == null)
                    this["InstanceCount"] = new Statistics<int>("InstanceCount");

                return ((Statistics<int>)this["InstanceCount"]).Value;
            }
        }

        #endregion Public Properties
    }
}