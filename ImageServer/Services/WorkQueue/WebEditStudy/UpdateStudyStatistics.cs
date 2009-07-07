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