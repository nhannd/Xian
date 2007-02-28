using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ClearCanvas.Common.Utilities;
using System.Reflection;
using System.Xml;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Base class for coded-value types.  A coded-value is similar to enumeration.  The main difference is that,
    /// whereas a C# enum declares a static set of values, a set of coded-values may be defined either statically
    /// (via static instance members) or dynamically through an XML configuration.  Also, whereas an enum defines
    /// a set of singular values, each value in a set of coded values has an associated code and an optional description.
    /// The set of coded-values is accessed through the static Dictionary property of the coded-value class.
    /// </summary>
    public abstract class CodedValue : DomainObject, IEquatable<CodedValue>
    {
        private string _code;
        private string _value;
        private string _description;
        private int _ordinal;
        private bool _active;

        /// <summary>
        /// Constructor required by nHibernate
        /// </summary>
        public CodedValue()
        {
        }

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        protected CodedValue(string code, string value, string description)
        {
            _code = code;
            _value = value;
            _description = description;
            _active = true;     // all static values are active
        }

        /// <summary>
        /// Gets the code
        /// </summary>
        public virtual string Code
        {
            get { return _code; }
            private set { _code = value; }
        }

        /// <summary>
        /// Gets the value
        /// </summary>
        public virtual string Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        /// <summary>
        /// Gets a description of the value
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
            private set { _description = value; }
        }

        public virtual int Ordinal
        {
            get { return _ordinal; }
            private set { _ordinal = value; }
        }

        /// <summary>
        /// True if the value is considered "active"
        /// </summary>
        public virtual bool Active
        {
            get { return _active; }
            private set { _active = value; }
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
            return this.Equals(obj as CodedValue);
        }

        /// <summary>
        /// Overridden to provide value-based equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(CodedValue x, CodedValue y)
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
        public static bool operator !=(CodedValue x, CodedValue y)
        {
            return !(x == y);
        }


        #region IEquatable<CodedValue> Members

        public bool Equals(CodedValue other)
        {
            return other != null && this.GetType().Equals(other.GetType()) && other.Code == this.Code;
        }

        #endregion
    }
}
