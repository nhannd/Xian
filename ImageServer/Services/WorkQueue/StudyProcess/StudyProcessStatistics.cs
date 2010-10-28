#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Store performance statistics of a study processor.
    /// </summary>
    public class StudyProcessStatistics : StatisticsSet
    {
        #region Public Properties

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

        public string Modality
        {
            set { this["Modality"] = new Statistics<string>("Modality", value); }
            get
            {
                if (this["Modality"] == null)
                    this["Modality"] = new Statistics<string>("Modality");

                return (this["Modality"] as Statistics<string>).Value;
            }
        }


        public int NumInstances
        {
            set { this["NumInstances"] = new Statistics<int>("NumInstances", value); }
            get
            {
                if (this["NumInstances"] == null)
                    this["NumInstances"] = new Statistics<int>("Modality");

                return (this["NumInstances"] as Statistics<int>).Value;
            }
        }

        public TimeSpanStatistics TotalProcessTime
        {
            get
            {
                if (this["TotalProcessTime"] == null)
                    this["TotalProcessTime"] = new TimeSpanStatistics("TotalProcessTime");

                return (this["TotalProcessTime"] as TimeSpanStatistics);
            }
            set { this["TotalProcessTime"] = value; }
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


        public TimeSpanStatistics StudyXmlLoadTime
        {
            get
            {
                if (this["StudyXmlLoadTime"] == null)
                    this["StudyXmlLoadTime"] = new TimeSpanStatistics("StudyXmlLoadTime");

                return (this["StudyXmlLoadTime"] as TimeSpanStatistics);
            }
            set { this["StudyXmlLoadTime"] = value; }
        }

        public TimeSpanStatistics StorageLocationLoadTime
        {
            get
            {
                if (this["StorageLocationLoadTime"] == null)
                    this["StorageLocationLoadTime"] = new TimeSpanStatistics("StorageLocationLoadTime");

                return (this["StorageLocationLoadTime"] as TimeSpanStatistics);
            }
            set { this["StorageLocationLoadTime"] = value; }
        }

        public TimeSpanStatistics UidsLoadTime
        {
            get
            {
                if (this["UidsLoadTime"] == null)
                    this["UidsLoadTime"] = new TimeSpanStatistics("UidsLoadTime");

                return (this["UidsLoadTime"] as TimeSpanStatistics);
            }
            set { this["UidsLoadTime"] = value; }
        }

        public TimeSpanStatistics SopProcessedEngineLoadTime
        {
            get
            {
                if (this["SopProcessedEngineLoadTime"] == null)
                    this["SopProcessedEngineLoadTime"] = new TimeSpanStatistics("SopProcessedEngineLoadTime");

                return (this["SopProcessedEngineLoadTime"] as TimeSpanStatistics);
            }
            set { this["SopProcessedEngineLoadTime"] = value; }
        }


        #endregion Public Properties

        #region Constructors

        public StudyProcessStatistics() : this("StudyProcess")
        {
        }


        public StudyProcessStatistics(string name)
            : base(name)
        {
        }

        #endregion Constructors
    }
}
