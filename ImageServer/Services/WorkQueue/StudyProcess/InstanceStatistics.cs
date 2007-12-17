#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
            : this("InstanceStatistics")
        {
        }

        public InstanceStatistics(string name)
            : base(name)
        {
            AddField(new TimeSpanStatistics("ProcessTime"));
            AddField(new ByteCountStatistics("FileSize"));
            AddField(new TimeSpanStatistics("FileLoadTime"));
            AddField(new TimeSpanStatistics("EngineLoadTime"));
            AddField(new TimeSpanStatistics("EngineExecutionTime"));
            AddField(new TimeSpanStatistics("InsertStreamTime"));
            AddField(new TimeSpanStatistics("InsertDBTime"));
        }

        #endregion Constructors

        #region Public Properties

        public TimeSpanStatistics ProcessTime
        {
            get { return (this["ProcessTime"] as TimeSpanStatistics); }
            set { this["ProcessTime"] = value; }
        }

        public ulong FileSize
        {
            set { this["FileSize"] = new ByteCountStatistics("FileSize", value); }
            get { return (this["FileSize"] as ByteCountStatistics).Value; }
        }

        public TimeSpanStatistics EngineLoadTime
        {
            get { return (this["EngineLoadTime"] as TimeSpanStatistics); }
            set { this["EngineLoadTime"] = value; }
        }

        public TimeSpanStatistics EngineExecutionTime
        {
            get { return (this["EngineExecutionTime"] as TimeSpanStatistics); }
            set { this["EngineExecutionTime"] = value; }
        }

        public TimeSpanStatistics InsertStreamTime
        {
            get { return (this["InsertStreamTime"] as TimeSpanStatistics); }
            set { this["InsertStreamTime"] = value; }
        }

        public TimeSpanStatistics InsertDBTime
        {
            get { return (this["InsertDBTime"] as TimeSpanStatistics); }
            set { this["InsertDBTime"] = value; }
        }

        public TimeSpanStatistics FileLoadTime
        {
            get { return (this["FileLoadTime"] as TimeSpanStatistics); }
            set { this["FileLoadTime"] = value; }
        }

        #endregion Public Properties
    }
}
