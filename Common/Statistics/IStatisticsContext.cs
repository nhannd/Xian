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
    /// Defines the interface of the context where performance <see cref="IStatistics"/> is being tracked.
    /// </summary>
    public interface IStatisticsContext
    {
        /// <summary>
        /// Gets or sets the ID of the context
        /// </summary>
        string ID { get; set; }
    }
}