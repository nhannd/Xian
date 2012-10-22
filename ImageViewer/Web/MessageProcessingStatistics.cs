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
