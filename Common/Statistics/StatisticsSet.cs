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

using System.Collections.Generic;
using System.Xml;
using System;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Statistics to hold one of more <see cref="IStatistics"/>.
    /// </summary>
    public class StatisticsSet
    {
        #region Protected Members

        protected String _name;
         // list of sub-statistics
        protected Dictionary<object, IStatistics> _fields = new Dictionary<object, IStatistics>();

        private Dictionary<object, StatisticsSet> _subStatistics = new Dictionary<object, StatisticsSet>();
       
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the statistics set.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets the statistics fields in the set.
        /// </summary>
        public ICollection<IStatistics> Fields
        {
            get { return _fields.Values; }
        }

        public Dictionary<object, StatisticsSet> SubStatistics
        {
            get { return _subStatistics; }
            set { _subStatistics = value; }
        }

        /// <summary>
        /// Gets or sets the statistics field in the set based on a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IStatistics this[object key]
        {
            get { return _fields[key]; }
            set { _fields[key] = value; }
        }

        #endregion

        #region Constructors

        public StatisticsSet(string name)
        {
            _name = name;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a specified statistics into the set using its name as the key.
        /// </summary>
        /// <param name="stat"></param>
        public void AddField(IStatistics stat)
        {
            _fields[stat.Name] = stat;
        }


        /// <summary>
        /// Adds a specified statistics into the set using its name as the key.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddField(string name, string value)
        {
            Statistics<string> newField = new Statistics<string>(name);
            newField.Value = value;
            _fields[name] = newField;
        }


        /// <summary>
        /// Adds a sub-statistics.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stat"></param>
        public void AddSubStats(object key, StatisticsSet stat)
        {
            _subStatistics.Add(key, stat);
        }

        /// <summary>
        /// Gets the XML representation of the statistics set.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public virtual XmlElement GetXmlElement(XmlDocument doc, bool recursive)
        {
            XmlElement el = doc.CreateElement("Statistics");

            if (this.GetType()!=typeof(StatisticsSet))
            {
                XmlAttribute attrType = doc.CreateAttribute("Type");
                attrType.Value = GetType().Name;
                el.Attributes.Append(attrType);    
            }

            XmlAttribute attrDescription = doc.CreateAttribute("Description");
            attrDescription.Value = Name;
            el.Attributes.Append(attrDescription);

            foreach (IStatistics field in Fields)
            {
                XmlAttribute[] attrValues = field.GetXmlAttributes(doc);
                foreach (XmlAttribute a in attrValues)
                {
                    el.Attributes.Append(a);
                }
            }

            if (recursive)
            {
                foreach (StatisticsSet substat in SubStatistics.Values)
                {
                    el.AppendChild(substat.GetXmlElement(doc, recursive));
                } 
            }
            

            return el;
        }

        #endregion

        
    }
}
