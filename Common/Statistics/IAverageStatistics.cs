#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Defines the interface of an average statistics
    /// </summary>
    public interface IAverageStatistics : IStatistics
    {
        /// <summary>
        /// Adds a sample
        /// </summary>
        /// <typeparam name="TSample">Type of the statistics sample</typeparam>
        /// <param name="sample"></param>
        void AddSample<TSample>(TSample sample);
    }
}