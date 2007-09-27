using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// When applied to a string-typed property of a domain object class, indicates that the property value
    /// must satisfy certain minimum and maximum constraints on the string length.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class LengthAttribute : Attribute
    {
        private int _min;
        private int _max;

        /// <summary>
        /// Constructor that accepts a minimum and maximum length value.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public LengthAttribute(int min, int max)
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Constructor that accepts a maximum length value (the minimum value is set to zero).
        /// </summary>
        /// <param name="max"></param>
        public LengthAttribute(int max)
            :this(0, max)
        {
        }

        public int Min { get { return _min; } }
        public int Max { get { return _max; } }
    }
}
