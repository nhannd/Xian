using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// A value stored in an instance of a <see cref="EnumTable"/>.
    /// </summary>
    /// <typeparam name="TEnum">The C# enum type on which this value is based</typeparam>
    public class EnumValue<e>
        where e : struct
    {
        private string _value;
        private string _description;
        private e _code;

        /// <summary>
        /// Used by an implementation of <see cref="IEnumBroker"/> to add entry to an <see cref="EnumTable"/>.
        /// Client code will never directly construct an instance of this class.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        public EnumValue()
        {
        }

        /// <summary>
        /// The C# enum value
        /// </summary>
        public e Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// The text corresponding to this enum value, used for display purposes.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// A description of the meaning of this enum value, if one was provided.  May be useful
        /// for display purposes.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Returns the <see cref="Value"/> property of this object, so that objects
        /// of this class can be passed directly to the presentation layer and will display correctly.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // to use this object in combo boxes, return the value here
            return this.Value;
        }

        /// <summary>
        /// Overridden to provide value-based hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Code.GetHashCode();
        }

        /// <summary>
        /// Overridden to provide value-based equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is EnumValue<e> && ((EnumValue<e>)obj) == this;
        }

        /// <summary>
        /// Overridden to provide value-based equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(EnumValue<e> x, EnumValue<e> y)
        {
            return x.Code.Equals(y.Code);
        }

        /// <summary>
        /// Overridden to provide value-based equality.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(EnumValue<e> x, EnumValue<e> y)
        {
            return !(x == y);
        }
    }
}
