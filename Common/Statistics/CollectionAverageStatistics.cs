#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#pragma warning disable 1591

using System;
using System.Collections.Generic;
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
        where T : StatisticsSet, new()
    {
        #region Private members

        private readonly StatisticsSetCollection<T> _collection = new StatisticsSetCollection<T>();
        
        #endregion Private members

        #region Public properties

        public int Count
        {
            get { return _collection.Count; }
        }

        #endregion Public properties

        #region Constructors

        public CollectionAverageStatistics(string name)
            : base(name)
        {
        }

        #endregion Constructors

        #region Protected Methods

        
        protected void CalculateAverage()
        {
            if (Count == 0)
                return;

            StatisticsSet firstItem = _collection.Items[0];

            foreach (IStatistics field in firstItem.Fields)
            {
                CalculateFieldAverage(field.Name);
            }
        }

        protected void CalculateFieldAverage(string fieldName)
        {
            string name = "Average_" + fieldName;
            double sum = 0;

            IStatistics field = _collection.Items[0][fieldName];

            IStatistics average = null;

            if (field is Statistics<int>)
            {
                average = new Statistics<int>(name);
                (average as Statistics<int>).Unit = (field as Statistics<int>).Unit;
                (average as Statistics<int>).ValueFormatter = (field as Statistics<int>).ValueFormatter;
            }
            else if (field is Statistics<uint>)
            {
                average = new Statistics<uint>(name);
                (average as Statistics<uint>).Unit = (field as Statistics<uint>).Unit;
                (average as Statistics<uint>).ValueFormatter = (field as Statistics<uint>).ValueFormatter;
            }
            else if (field is Statistics<long>)
            {
                average = new Statistics<long>(name);
                (average as Statistics<long>).Unit = (field as Statistics<long>).Unit;
                (average as Statistics<long>).ValueFormatter = (field as Statistics<long>).ValueFormatter;
            }

            else if (field is Statistics<ulong>)
            {
                average = new Statistics<ulong>(name);
                (average as Statistics<ulong>).Unit = (field as Statistics<ulong>).Unit;
                (average as Statistics<ulong>).ValueFormatter = (field as Statistics<ulong>).ValueFormatter;
            }
            else if (field is Statistics<double>)
            {
                average = new Statistics<double>(name);
                (average as Statistics<double>).Unit = (field as Statistics<double>).Unit;
                (average as Statistics<double>).ValueFormatter = (field as Statistics<double>).ValueFormatter;
            }
            else if (field is TimeSpanStatistics)
            {
                average = new TimeSpanStatistics(name);
                (average as TimeSpanStatistics).Unit = (field as TimeSpanStatistics).Unit;
                (average as TimeSpanStatistics).ValueFormatter = (field as TimeSpanStatistics).ValueFormatter;
            }
            else
            {
                // Not supported
                return;
            }


            foreach (T item in _collection.Items)
            {
                field = item[fieldName];

                if (field is Statistics<int>)
                {
                    sum += ((Statistics<int>)field).Value;
                }
                else if (field is Statistics<uint>)
                {
                    sum += ((Statistics<uint>)field).Value;
                }
                else if (field is Statistics<long>)
                {
                    sum += ((Statistics<long>)field).Value;
                }
                else if (field is Statistics<ulong>)
                {
                    sum += ((Statistics<ulong>)field).Value;
                }
                else if (field is Statistics<double>)
                {
                    sum += ((Statistics<double>)field).Value;
                }
                else if (field is TimeSpanStatistics)
                {
                    sum += ((TimeSpanStatistics)field).Value.Ticks;
                }
            }

            if (average is TimeSpanStatistics)
            {
                (average as TimeSpanStatistics).Value = new TimeSpan((long)sum / _collection.Count);
            }
            else if (average is Statistics<int>)
            {
                (average as Statistics<int>).Value = (int)sum / _collection.Count;
            }
            else if (average is Statistics<uint>)
            {
                (average as Statistics<uint>).Value = (uint)(sum / _collection.Count);
            }
            else if (average is Statistics<long>)
            {
                (average as Statistics<long>).Value = (long)(sum / _collection.Count);
            }
            else if (average is Statistics<ulong>)
            {
                (average as Statistics<ulong>).Value = (ulong)(sum / _collection.Count);
            }
            else if (average is Statistics<double>)
            {
                (average as Statistics<double>).Value = (sum / _collection.Count);
            }


            AddField(average);
        }


        #endregion Protected Methods

        #region Public methods

        /// <summary>
        /// Returns an instance of the underlying statistics set.
        /// </summary>
        /// <returns></returns>
        public T NewStatistics(string name)
        {
            return _collection.NewStatistics(name);
        }

        /// <summary>
        /// Returns the XML element which contains the average attributes for the child collection.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public override XmlElement GetXmlElement(XmlDocument doc, bool recursive)
        {
            CalculateAverage();
            XmlElement xml = base.GetXmlElement(doc, recursive);
            if (recursive)
            {
                List<XmlElement> list = _collection.ToXmlElements(doc, recursive);
                foreach (XmlElement el in list)
                {
                    xml.AppendChild(el);
                }
            }
            return xml;
        }

        #endregion Public methods
    }
}
