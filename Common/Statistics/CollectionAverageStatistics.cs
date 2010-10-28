#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Base statistics class that automatically calculates averages 
    /// of the underlying <see cref="StatisticsSetCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the statistics in the collection</typeparam>
    /// <remarks>
    /// The generated statistics contains fields with the average values of the corresponding fields in the collection.
    /// </remarks>
    public class CollectionAverageStatistics<T> : StatisticsSet
        where T : StatisticsSet
    {
        #region Private members

        #endregion Private members

        #region Public properties

        #endregion Public properties

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="CollectionAverageStatistics{T}"/> with a specified name.
        /// </summary>
        /// <param name="name"></param>
        public CollectionAverageStatistics(string name)
            : base(name, name)
        {
        }

        #endregion Constructors

        #region Private Methods

        #endregion

        #region Overridden Public Methods

        /// <summary>
        /// Returns the XML element which contains the average attributes for the child collection.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public override XmlElement GetXmlElement(XmlDocument doc, bool recursive)
        {
            CalculateAverage();
            return base.GetXmlElement(doc, recursive);
        }

        #endregion
    }
}