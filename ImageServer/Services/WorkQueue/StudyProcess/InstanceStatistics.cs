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

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Stores statistics of a WorkQueue instance processing.
    /// </summary>
    internal class InstanceStatistics : StatisticsSet
    {
        #region Constructors

        public InstanceStatistics()
            : this("Instance")
        {
        }

        public InstanceStatistics(string name)
            : base(name)
        {

        }

        #endregion Constructors

        #region Public Properties

        public TimeSpanStatistics ProcessTime
        {
            get
            {
                if (this["ProcessTime"]==null)
                    this["ProcessTime"] = new TimeSpanStatistics("ProcessTime");

                return (this["ProcessTime"] as TimeSpanStatistics);
            }
            set { this["ProcessTime"] = value; }
        }

        public TimeSpanStatistics DBUpdateTime
        {
            get
            {
                if (this["DBUpdateTime"] == null)
                    this["DBUpdateTime"] = new TimeSpanStatistics("DBUpdateTime");

                return (this["DBUpdateTime"] as TimeSpanStatistics);
            }
            set { this["DBUpdateTime"] = value; }
        }

        public ulong FileSize
        {
            set
            {
                this["FileSize"] = new ByteCountStatistics("FileSize", value);
            }
            get
            {
                if (this["FileSize"] == null)
                    this["FileSize"] = new ByteCountStatistics("FileSize");

                return (this["FileSize"] as ByteCountStatistics).Value;
            }
        }

        public TimeSpanStatistics SopRulesLoadTime
        {
            get
            {
                if (this["SopRulesLoadTime"] == null)
                    this["SopRulesLoadTime"] = new TimeSpanStatistics("SopRulesLoadTime");

                return (this["SopRulesLoadTime"] as TimeSpanStatistics);
            }
            set { this["SopRulesLoadTime"] = value; }
        }

        public TimeSpanStatistics SeriesRulesLoadTime
        {
            get
            {
                if (this["SeriesRulesLoadTime"] == null)
                    this["SeriesRulesLoadTime"] = new TimeSpanStatistics("SeriesRulesLoadTime");

                return (this["SeriesRulesLoadTime"] as TimeSpanStatistics);
            }
            set { this["SeriesRulesLoadTime"] = value; }
        }


        public TimeSpanStatistics StudyRulesLoadTime
        {
            get
            {
                if (this["StudyRulesLoadTime"] == null)
                    this["StudyRulesLoadTime"] = new TimeSpanStatistics("StudyRulesLoadTime");

                return (this["StudyRulesLoadTime"] as TimeSpanStatistics);
            }
            set { this["StudyRulesLoadTime"] = value; }
        }


        public TimeSpanStatistics SopEngineExecutionTime
        {
            get
            {
                if (this["SopEngineExecutionTime"] == null)
                    this["SopEngineExecutionTime"] = new TimeSpanStatistics("SopEngineExecutionTime");

                return (this["SopEngineExecutionTime"] as TimeSpanStatistics);
            }
            set { this["SopEngineExecutionTime"] = value; }
        }

        public TimeSpanStatistics SeriesEngineExecutionTime
        {
            get
            {
                if (this["SeriesEngineExecutionTime"] == null)
                    this["SeriesEngineExecutionTime"] = new TimeSpanStatistics("SeriesEngineExecutionTime");

                return (this["SeriesEngineExecutionTime"] as TimeSpanStatistics);
            }
            set { this["SeriesEngineExecutionTime"] = value; }
        }

        public TimeSpanStatistics StudyEngineExecutionTime
        {
            get
            {
                if (this["StudyEngineExecutionTime"] == null)
                    this["StudyEngineExecutionTime"] = new TimeSpanStatistics("StudyEngineExecutionTime");

                return (this["StudyEngineExecutionTime"] as TimeSpanStatistics);
            }
            set { this["StudyEngineExecutionTime"] = value; }
        }

        public TimeSpanStatistics InsertStreamTime
        {
            get
            {
                if (this["InsertStreamTime"] == null)
                    this["InsertStreamTime"] = new TimeSpanStatistics("InsertStreamTime");

                return (this["InsertStreamTime"] as TimeSpanStatistics);
            }
            set { this["InsertStreamTime"] = value; }
        }

        public TimeSpanStatistics InsertDBTime
        {
            get
            {
                if (this["InsertDBTime"] == null)
                    this["InsertDBTime"] = new TimeSpanStatistics("InsertDBTime");

                return (this["InsertDBTime"] as TimeSpanStatistics);
            }
            set { this["InsertDBTime"] = value; }
        }

        public TimeSpanStatistics FileLoadTime
        {
            get
            {
                if (this["FileLoadTime"] == null)
                    this["FileLoadTime"] = new TimeSpanStatistics("FileLoadTime");

                return (this["FileLoadTime"] as TimeSpanStatistics);
            }
            set { this["FileLoadTime"] = value; }
        }

        #endregion Public Properties
    }
}
