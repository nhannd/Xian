#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CompressStudy
{
	class CompressStudyStatistics : StatisticsSet
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

                return ((Statistics<string>) this["StudyInstanceUid"]).Value;
            }
        }

        public string Modality
        {
            set { this["Modality"] = new Statistics<string>("Modality", value); }
            get
            {
                if (this["Modality"] == null)
                    this["Modality"] = new Statistics<string>("Modality");

                return ((Statistics<string>) this["Modality"]).Value;
            }
        }


        public int NumInstances
        {
            set { this["NumInstances"] = new Statistics<int>("NumInstances", value); }
            get
            {
                if (this["NumInstances"] == null)
                    this["NumInstances"] = new Statistics<int>("Modality");

                return ((Statistics<int>) this["NumInstances"]).Value;
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

        #endregion Public Properties

        #region Constructors

        public CompressStudyStatistics() : this("CompressStudy")
        {
        }


		public CompressStudyStatistics(string name)
            : base(name)
        {
        }

        #endregion Constructors
	}
}
