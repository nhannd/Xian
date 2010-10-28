#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
        /// Transmission speed.
        /// </summary>
        public RateStatistics TransmissionSpeed
        {
            get
            {
                if (this["TransmissionSpeed"] == null)
                    this["TransmissionSpeed"] = new RateStatistics("TransmissionSpeed", RateType.BYTES);

                return (this["TransmissionSpeed"] as RateStatistics);
            }
            set { this["TransmissionSpeed"] = value; }
        }
        #endregion
    }
    
}
