#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming
{
    internal class HeaderStreamingStatistics : StatisticsSet
    {
        #region Constructors

        public HeaderStreamingStatistics()
            : base("HeaderStreaming")
        {
            AddField(new Statistics<string>("Client"));
            AddField(new Statistics<string>("StudyInstanceUid"));
            AddField(new TimeSpanStatistics("ProcessTime"));
        }

        #endregion Constructors

        #region Public Properties

        public TimeSpanStatistics ProcessTime
        {
            get { return this["ProcessTime"] as TimeSpanStatistics; }
        }

        #endregion Public Properties
    }
}