#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Represents the statistics generates by <see cref="WADORequestProcessor"/>
    /// </summary>
    public class WADORequestProcessorStatistics : StatisticsSet
    {
        #region Constructors
        public WADORequestProcessorStatistics(string name)
            : base(name)
        {
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Total process time.
        /// </summary>
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

        /// <summary>
        /// Total transmission time.
        /// </summary>
        public TimeSpanStatistics TransmissionTime
        {
            get
            {
                if (this["TransmissionTime"] == null)
                    this["TransmissionTime"] = new TimeSpanStatistics("TransmissionTime");

                return (this["TransmissionTime"] as TimeSpanStatistics);
            }
            set { this["TransmissionTime"] = value; }
        }

        /// <summary>
        /// Image size
        /// </summary>
        public ulong ImageSize
        {
            set
            {
                this["ImageSize"] = new ByteCountStatistics("ImageSize", value);
            }
            get
            {
                if (this["ImageSize"] == null)
                    this["ImageSize"] = new ByteCountStatistics("ImageSize");

                return (this["ImageSize"] as ByteCountStatistics).Value;
            }
        }

        /// <summary>
        /// Read buffer size used
        /// </summary>
        public ulong BufferSize
        {
            set
            {
                this["BufferSize"] = new ByteCountStatistics("BufferSize", value);
            }
            get
            {
                if (this["BufferSize"] == null)
                    this["BufferSize"] = new ByteCountStatistics("BufferSize");

                return (this["BufferSize"] as ByteCountStatistics).Value;
            }
        }

        /// <summary>
        /// Number of disk accesses (reading).
        /// </summary>
        public int DiskAccessCount
        {
            set
            {
                this["DiskAccessCount"] = new Statistics<int>("DiskAccessCount", value);
            }
            get
            {
                if (this["DiskAccessCount"] == null)
                    this["DiskAccessCount"] = new Statistics<int>("DiskAccessCount");

                return (this["DiskAccessCount"] as Statistics<int>).Value;
            }
        }

        /// <summary>
        /// Number of network write accesses (send)
        /// </summary>
        public int NetworkWriteCount
        {
            set
            {
                this["NetworkWriteCount"] = new Statistics<int>("NetworkWriteCount", value);
            }
            get
            {
                if (this["NetworkWriteCount"] == null)
                    this["NetworkWriteCount"] = new Statistics<int>("NetworkWriteCount");

                return (this["NetworkWriteCount"] as Statistics<int>).Value;
            }
        }


        /// <summary>
        /// Total transmission time.
        /// </summary>
        public AverageRateStatistics AverageDiskSpeed
        {
            get
            {
                if (this["AverageDiskSpeed"] == null)
                    this["AverageDiskSpeed"] = new AverageRateStatistics("AverageDiskSpeed", RateType.BYTES);

                return (this["AverageDiskSpeed"] as AverageRateStatistics);
            }
            set { this["AverageDiskSpeed"] = value; }
        }


        /// <summary>
        /// Total transmission time.
        /// </summary>
        public AverageRateStatistics AverageTransmissionSpeed
        {
            get
            {
                if (this["AverageTransmissionSpeed"] == null)
                    this["AverageTransmissionSpeed"] = new AverageRateStatistics("AverageTransmissionSpeed", RateType.BYTES);

                return (this["AverageTransmissionSpeed"] as AverageRateStatistics);
            }
            set { this["AverageTransmissionSpeed"] = value; }
        }
        #endregion
    }
    
}
