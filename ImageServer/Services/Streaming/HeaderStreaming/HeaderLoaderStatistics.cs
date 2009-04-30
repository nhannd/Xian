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

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming
{
    internal class HeaderLoaderStatistics : StatisticsSet
    {
        #region Constructors

        public HeaderLoaderStatistics()
            : base("HeaderLoading")
        {
            AddField(new ByteCountStatistics("HeaderSize"));
            AddField(new TimeSpanStatistics("FindStudyFolder"));
            AddField(new TimeSpanStatistics("CompressHeader"));
            AddField(new TimeSpanStatistics("LoadHeaderStream"));
        }

        #endregion Constructors

        #region Public Properties

        public ulong Size
        {
            get { return (this["HeaderSize"] as ByteCountStatistics).Value; }
            set { (this["HeaderSize"] as ByteCountStatistics).Value = value; }
        }

        public TimeSpanStatistics FindStudyFolder
        {
            get { return this["FindStudyFolder"] as TimeSpanStatistics; }
            set { this["FindStudyFolder"] = value; }
        }

        public TimeSpanStatistics LoadHeaderStream
        {
            get { return this["LoadHeaderStream"] as TimeSpanStatistics; }
            set { this["LoadHeaderStream"] = value; }
        }

        public TimeSpanStatistics CompressHeader
        {
            get { return this["CompressHeader"] as TimeSpanStatistics; }
            set { this["CompressHeader"] = value; }
        }

        #endregion Public Properties
    }
}