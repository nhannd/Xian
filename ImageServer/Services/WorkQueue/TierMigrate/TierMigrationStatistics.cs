#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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

        public TimeSpanStatistics CopyFiles
        {
            get
            {
                if (this["CopyFiles"] == null)
                    this["CopyFiles"] = new TimeSpanStatistics("CopyFiles");

                return (this["CopyFiles"] as TimeSpanStatistics);
            }
            set { this["CopyFiles"] = value; }
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
