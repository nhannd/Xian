#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageViewer.Web
{
    public class MessageProcessingStatistics : StatisticsSet
    {
        #region Constructors

        public MessageProcessingStatistics(Type messageType)
            : base("MessageProcessing", messageType.Name)
        {
        }

        #endregion Constructors

        #region Public Properties

        public TimeSpanStatistics ProcessingTime
        {
            get
            {
                if (this["ProcessingTime"] == null)
                {
                    this["ProcessingTime"] = new TimeSpanStatistics("ProcessingTime");
                }
                return this["ProcessingTime"] as TimeSpanStatistics;
            }
            set
            {
                this["ProcessingTime"] = value;
            }
        }

        public TimeSpanStatistics TimeSinceLastMessageProcessed
        {
            get
            {
                if (this["TimeSinceLastMessageProcessed"] == null)
                {
                    this["TimeSinceLastMessageProcessed"] = new TimeSpanStatistics("TimeSinceLastMessageProcessed");
                }
                return this["TimeSinceLastMessageProcessed"] as TimeSpanStatistics;
            }
            set
            {
                this["TimeSinceLastMessageProcessed"] = value;
            }
        }

        #endregion Public Properties
    }
}
