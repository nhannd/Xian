#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Defines the interface of a statistics object.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface IStatistics : ICloneable
    {
        /// <summary>
        /// Sets or gets the context of the statistics
        /// </summary>
        IStatisticsContext Context { set; get; }

        /// <summary>
        /// Gets the name of the statistics
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the unit of the statistics value.
        /// </summary>
        string Unit { get; set; }


        /// <summary>
        /// Gets the formatted string representation of the value.
        /// </summary>
        string FormattedValue { get; }

        /// <summary>
        /// Gets the XML attribute representation of the statistics.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        XmlAttribute[] GetXmlAttributes(XmlDocument doc);

        /// <summary>
        /// Gets a new statistics instance that can be used to generate and store the average of current statistics 
        /// </summary>
        /// <returns></returns>
        IAverageStatistics NewAverageStatistics();
    }
}