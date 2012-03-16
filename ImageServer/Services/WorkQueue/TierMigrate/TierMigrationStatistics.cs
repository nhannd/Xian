#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.TierMigrate
{
    internal class TierMigrationStatistics : StatisticsSet
    {
        public TierMigrationStatistics()
            : base("TierMigrationStatistics")
        {
        }
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

                return (this["StudySize"] as ByteCountStatistics).Value;
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

        public TimeSpanStatistics DBUpdate
        {
            get
            {
                if (this["DBUpdate"] == null)
                    this["DBUpdate"] = new TimeSpanStatistics("DBUpdate");

                return (this["DBUpdate"] as TimeSpanStatistics);
            }
            set { this["DBUpdate"] = value; }
        }

        public RateStatistics CopyFiles
        {
            get
            {
                if (this["CopyFiles"] == null)
                    this["CopyFiles"] = new RateStatistics("CopyFiles", RateType.BYTES);

                return (this["CopyFiles"] as RateStatistics);
            }
            set { this["CopyFiles"] = value; }
        }

        public TimeSpanStatistics DeleteDirTime
        {
            get
            {
                if (this["DeleteDirTime"] == null)
                    this["DeleteDirTime"] = new TimeSpanStatistics("DeleteDirTime");

                return (this["DeleteDirTime"] as TimeSpanStatistics);
            }
            set { this["DeleteDirTime"] = value; }
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
