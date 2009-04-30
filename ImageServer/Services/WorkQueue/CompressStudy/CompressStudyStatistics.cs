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
