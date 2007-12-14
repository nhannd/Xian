#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using System.Xml;
using System;
using ClearCanvas.Common.Performance;

namespace ClearCanvas.Common.Performance
{

    /// <summary>
    /// Statistics to hold one of more statistics.
    /// </summary>
    public abstract class StatisticsSet
    {

        #region Protected Members
        protected String _name;

        // list of sub-statistics
        protected Dictionary<object, IStatistics> _fields = new Dictionary<object, IStatistics>();
        #endregion

        #region Public Properties

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ICollection<IStatistics> Fields
        {
            get
            {
                return _fields.Values;
            }
        }

        #endregion

        #region Constructors

        public StatisticsSet(string name)
        {
            _name = name;
        }

        public IStatistics this[object key]
        {
            get
            {
                return _fields[key];
            }
            set
            {
                _fields[key] = value;
            }
        }
        #endregion



        #region Public Methods

       


        protected void AddField(IStatistics stat)
        {
            _fields[stat.Name] = stat;
        }

        

        #endregion



        #region Protected Methods

        #endregion



        #region Abstract Methods

        #endregion

        #region Overrides

        #endregion

        public virtual XmlElement GetXmlElement(XmlDocument doc)
        {
            //<Statistics name="xxxxx" >
            //     
            //</Statistics>
            XmlElement el = doc.CreateElement("Statistics");
            XmlAttribute attrType = doc.CreateAttribute("Type");
            attrType.Value = GetType().Name;
            el.Attributes.Append(attrType);

            foreach (IStatistics field in Fields)
            {
                XmlAttribute[] attrValues = field.GetXmlAttributes(doc);
                foreach (XmlAttribute a in attrValues)
                {
                    el.Attributes.Append(a);
                }

            }

            return el;
        }

        public override string ToString()
        {
            // spit out all properties as XML
            
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");

            XmlDocument doc = new XmlDocument();
            XmlElement el = this.GetXmlElement(doc);

            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                el.WriteTo(writer);
                writer.Flush();
            }
            return sb.ToString();
        }


    }


}
