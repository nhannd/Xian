using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess
{
    internal class FilesystemReprocessStatistics : StatisticsSet
    {
        #region Public Properties

        public string Filesystem
        {
            set { this["Filesystem"] = new Statistics<string>("Filesystem", value); }
            get { return (this["Filesystem"] as Statistics<string>).Value; }
        }

        public RateStatistics StudyRate
        {
            get { return this["StudyRate"] as RateStatistics; }
        }


        public int NumStudies
        {
            set { (this["NumStudies"] as Statistics<int>).Value = value; }
            get { return (this["NumStudies"] as Statistics<int>).Value; }
        }

        #endregion Public Properties

        #region Constructors

        public FilesystemReprocessStatistics()
            : base("FilesystemReprocess")
        {
            AddField(new Statistics<string>("Filesystem"));
            AddField(new Statistics<int>("NumStudies"));
            AddField(new RateStatistics("StudyRate", "studies"));
        }

        #endregion Constructors
    }
}
