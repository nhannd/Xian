using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    public class WADORequestProcessorStatistics : StatisticsSet
    {
        public WADORequestProcessorStatistics(string name)
            : base(name)
        {
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
    }
    
}
