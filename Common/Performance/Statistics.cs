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

using System;
using System.Collections.Generic;
using System.Xml;


namespace ClearCanvas.Common.Performance
{

    /// <summary>
    /// Generic base statistics class that implements <see cref="IStatistics"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Statistics<T> : IStatistics
    {
        private readonly string _name;
        private T _value;
        private string _unit;

        public delegate string ValueFormatterDelegate(T value);
        
        private ValueFormatterDelegate _formatter;
        
        public Statistics(string name)
        {
            _name = name;
        }

        

        public Statistics(string name, ValueFormatterDelegate formatter)
        {
            _name = name;
            _formatter = formatter;
        }

        public Statistics(string name, T value)
        {
            _name = name;
            _value = value;
        }

        public static explicit operator T(Statistics<T> stat)
        {
            return stat.Value;
        }



        public string Name
        {
            get { return _name; }
        }

        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }


        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        #region IStatistics Members

        
        public XmlAttribute[] GetXmlAttributes(XmlDocument doc)
        {
            List<XmlAttribute> list = new List<XmlAttribute>();

            XmlAttribute attr = doc.CreateAttribute(Name);
            if (_value != null)
                attr.Value = FormattedValue;
            list.Add(attr);

            return list.ToArray();
        }

        #endregion

        #region IStatistics Members


        public virtual bool SetValue(object value)
        {
            _value = (T) value;
            return true;   
        }

        #endregion

        #region IStatistics Members

        public virtual string FormattedValue
        {
            get
            {
                return FormatValue(_value);
            }
        }

        protected string FormatValue(object obj)
        {
            if (ValueFormatter != null && obj is T)
                return ValueFormatter((T)obj);

            if (_unit != null)
                return String.Format("{0} {1}", obj, _unit);
            else
                return obj.ToString();  
        }

        public ValueFormatterDelegate ValueFormatter
        {
            get { return _formatter; }
            set { _formatter = value; }
        }

        #endregion
    }

}
