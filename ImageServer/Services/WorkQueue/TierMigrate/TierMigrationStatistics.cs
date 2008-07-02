using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    internal class TierMigrationStatistics : StatisticsSet
    {
        public string StudyInstanceUid
        {
            set
            {
                this["StudyInstanceUid"] = new Statistics<string>("StudyInstanceUid", value);
            }
            get
            {
                if (this["StudyInstanceUid"] == null)
                    this["StudyInstanceUid"] = new Statistics<string>("StudyInstanceUid");

                return (this["StudyInstanceUid"] as Statistics<string>).Value;
            }
        }

        public RateStatistics ProcessSpeed
        {
            get
            {
                if (this["ProcessSpeed"] == null)
                    this["ProcessSpeed"] = new RateStatistics("ProcessSpeed", RateType.BYTES);

                return (this["ProcessSpeed"] as RateStatistics);
            }
            set { this["ProcessSpeed"] = value; }
        }

    }

    internal class TierMigrationAverageStatistics : StatisticsSet
    {
        public TierMigrationAverageStatistics()
            :base("TierMigration", "Tier Migration Moving Average")
        {
            
        }

        public AverageByteCountStatistics AverageStudySize
        {
            get
            {
                if (this["AverageStudySize"] == null)
                    this["AverageStudySize"] = new AverageByteCountStatistics("AverageStudySize");

                return (this["AverageStudySize"] as AverageByteCountStatistics);
            }
            set { this["AverageStudySize"] = value; }
        }
        public AverageRateStatistics AverageProcessSpeed
        {
            get
            {
                if (this["AverageProcessSpeed"] == null)
                    this["AverageProcessSpeed"] = new AverageRateStatistics("AverageProcessSpeed", RateType.BYTES);

                return (this["AverageProcessSpeed"] as AverageRateStatistics);
            }
            set { this["AverageProcessSpeed"] = value; }
        }

        public AverageTimeSpanStatistics AverageFileMoveTime
        {
            get
            {
                if (this["AverageFileMoveTime"] == null)
                    this["AverageFileMoveTime"] = new AverageTimeSpanStatistics("AverageFileMoveTime");

                return (this["AverageFileMoveTime"] as AverageTimeSpanStatistics);
            }
            set { this["AverageFileMoveTime"] = value; }
        }

        public AverageTimeSpanStatistics AverageDBUpdateTime
        {
            get
            {
                if (this["AverageDBUpdateTime"] == null)
                    this["AverageDBUpdateTime"] = new AverageTimeSpanStatistics("AverageDBUpdateTime");

                return (this["AverageDBUpdateTime"] as AverageTimeSpanStatistics);
            }
            set { this["AverageDBUpdateTime"] = value; }
        }

    }
}
