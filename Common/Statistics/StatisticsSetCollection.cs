#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System.Collections.Generic;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Base collection of <see cref="StatisticsSet"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class StatisticsSetCollection<T>
        where T : StatisticsSet, new()
    {
        #region Private members

        private List<T> _list = new List<T>();

        #endregion Private members

        #region public properties

        public List<T> Items
        {
            get { return _list; }
            set { _list = value; }
        }

        public int Count
        {
            get { return Items.Count; }
        }

        #endregion public properties

        #region public methods

        ///// <summary>
        ///// Returns a new instance of the underlying statistics set.
        ///// </summary>
        ///// <param name="name">Name to be assigned to the statistics set.</param>
        ///// <returns></returns>
        //public T NewStatistics(string name)
        //{
        //    T newStat = new T();
        //    newStat.Name = name;
        //    _list.Add(newStat);
        //    return newStat;
        //}

        /// <summary>
        /// Returns the statistics collection as a list of XML elements.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public virtual List<XmlElement> ToXmlElements(XmlDocument doc, bool recursive)
        {
            List<XmlElement> list = new List<XmlElement>();

            foreach (StatisticsSet item in Items)
            {
                XmlElement xml = item.GetXmlElement(doc, recursive);
                list.Add(xml);
            }

            return list;
        }

        #endregion public methods
    }
}