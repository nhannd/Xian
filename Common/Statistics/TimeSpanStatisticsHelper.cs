#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Provides helper method to generate <see cref="TimeSpanStatistics"/>
    /// </summary>
    public class TimeSpanStatisticsHelper
    {
        /// <summary>
        /// Defines the delegate to a code block whose execution time will be measured using <see cref="Measure"/>.
        /// </summary>
        public delegate void ExecutationBlock();

        /// <summary>
        /// Measures the elapsed time taken by an code block.
        /// </summary>
        /// <param name="executionBlock">Delegate to the code block which will be executed by this method and its elapsed will be measured</param>
        /// <returns>The <see cref="TimeSpanStatistics"/> object contains the elapsed time of the execution</returns>
        static public TimeSpanStatistics Measure(ExecutationBlock executionBlock)
        {
            TimeSpanStatistics stat = new TimeSpanStatistics();

            stat.Start();
            try
            {
                executionBlock();
                return stat;
            }
            finally
            {
                stat.End();
            }
        }
    }
}
