using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ClearCanvas.Common.Utilities;
using System.Reflection;
using System.Xml;

namespace ClearCanvas.Enterprise
{
    public abstract class CodedValue
    {
        private string _code;
        private string _value;
        private string _description;
        private bool _active;

        private bool _isStatic;

        /// <summary>
        /// Constructor used by the framework to instantiate dynamic values
        /// </summary>
        public CodedValue()
        {
            _isStatic = false;
        }

        /// <summary>
        /// Constructor used to create static values
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        protected CodedValue(string code, string value, string description)
        {
            _code = code;
            _value = value;
            _description = description;
            _isStatic = true;   // any value created by this constructor is static
            _active = true;     // all static values are active
        }

        /// <summary>
        /// Gets the code
        /// </summary>
        public string Code
        {
            get { return _code; }
            internal set { _code = value; }
        }

        /// <summary>
        /// Gets the value
        /// </summary>
        public string Value
        {
            get { return _value; }
            internal set { _value = value; }
        }

        /// <summary>
        /// Gets a description of the value
        /// </summary>
        public string Description
        {
            get { return _description; }
            internal set { _description = value; }
        }

        /// <summary>
        /// True if the value is considered "active"
        /// </summary>
        public bool Active
        {
            get { return _active; }
            internal set { _active = value; }
        }

        /// <summary>
        /// True if the value is a static instance
        /// </summary>
        public bool IsStatic
        {
            get { return _isStatic; }
        }

        /// <summary>
        /// Deserializes this value from XML
        /// </summary>
        /// <param name="cvElement"></param>
        internal void FromXml(XmlElement cvElement)
        {
            _code = cvElement.GetAttribute("code");
            _value = cvElement.GetAttribute("value");

            bool.TryParse(cvElement.GetAttribute("active"), out _active);

            foreach (XmlNode node in cvElement.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (node.Name == "description")
                        _description = node.Value;
                }
            }
        }
    }

    /// <summary>
    /// Base class for coded-value types.  A coded-value is similar to enumeration.  The main difference is that,
    /// whereas a C# enum declares a static set of values, a set of coded-values may be defined either statically
    /// (via static instance members) or dynamically through an XML configuration.  Also, whereas an enum defines
    /// a set of singular values, each value in a set of coded values has an associated code and an optional description.
    /// The set of coded-values is accessed through the static Dictionary property of the coded-value class.
    /// </summary>
    /// <typeparam name="TCodedValue">The type of the coded-value, which must also be the subclass of this class</typeparam>
    public abstract class CodedValue<TCodedValue> : CodedValue, IEquatable<TCodedValue>
        where TCodedValue : CodedValue<TCodedValue>, new()
    {
        private static CodedValueDictionary<TCodedValue> _dictionary;

        /// <summary>
        /// Gets the dictionary containing the set of all coded-values for this class
        /// </summary>
        public static CodedValueDictionary<TCodedValue> Dictionary
        {
            get
            {
                if (_dictionary == null)
                {
                    _dictionary = new CodedValueDictionary<TCodedValue>();
                }
                return _dictionary;
            }
        }

        /// <summary>
        /// Constructor used by the framework to construct instance dynamically
        /// </summary>
        public CodedValue()
        {
        }

        /// <summary>
        /// Constructor used to define static instances
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        protected CodedValue(string code, string value, string description)
            :base(code, value, description)
        {
        }

        /// <summary>
        /// Returns the <see cref="Code"/> property of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Code;
        }

        /// <summary>
        /// Overridden to provide value-based hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Code == null ? 0 : this.Code.GetHashCode();
        }

        /// <summary>
        /// Overridden to provide value-based equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as TCodedValue);
        }

        /// <summary>
        /// Overridden to provide value-based equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(CodedValue<TCodedValue> x, CodedValue<TCodedValue> y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if ((x as object) == null || (y as object) == null)
                return false;

            return x.Equals(y);
        }

        /// <summary>
        /// Overridden to provide value-based equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(CodedValue<TCodedValue> x, CodedValue<TCodedValue> y)
        {
            return !(x == y);
        }


        #region IEquatable<TCodedValue> Members

        public bool Equals(TCodedValue other)
        {
            return other != null && other.Code == this.Code;
        }

        #endregion
    }
}
