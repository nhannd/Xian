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
    /// Defines the interface to a component that listens to statistics logging event.
    /// </summary>
    public interface IStatisticsLoggerListener
    {
        /// <summary>
        /// Called when a statistics is logged.
        /// </summary>
        /// <param name="statistics">The logged statistics</param>
        void OnStatisticsLogged(StatisticsSet statistics);
    }
}