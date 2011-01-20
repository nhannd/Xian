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