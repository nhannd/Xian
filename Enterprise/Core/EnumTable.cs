using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Holds meta-data for a domain enumeration.  The table holds a set of <see cref="EnumValue"/> instances
    /// that hold meta-data about a C# enum, one instance for each possible value of the enum.
    /// </summary>
    /// <typeparam name="e">The C# enum that this table corresponds to</typeparam>
    /// <typeparam name="e">The <see cref="EnumValue"/> class that this table corresponds to</typeparam>
    public class EnumTable<e, E>
        where e : struct
        where E : EnumValue<e>, new()
    {
        private IDictionary<e, E> _values;
        private string[] _displayValues;


        private static IList<E> GetValuesFromAttributes()
        {
            List<E> values = new List<E>();
            foreach (FieldInfo fi in typeof(e).GetFields())
            {
                // try to get an attribute
                object[] attrs = fi.GetCustomAttributes(typeof(EnumValueAttribute), false);
                if (attrs.Length > 0)
                {
                    E value = new E();
                    value.Code = (e)fi.GetValue(null);
                    value.Value = ((EnumValueAttribute)attrs[0]).Value;
                    values.Add(value);
                }
            }
            return values;
        }

        public EnumTable()
            : this(GetValuesFromAttributes())
        {
        }

        public EnumTable(IList<E> values)
        {
            _values = new Dictionary<e, E>();
            foreach (E val in values)
            {
                _values.Add(val.Code, val);
            }
        }

        /// <summary>
        /// Returns the <see cref="EnumValue"/> for the specified C# enum member.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public E this[e code]
        {
            get { return _values[code]; }
        }

        /// <summary>
        /// Returns the <see cref="EnumValue"/> corresponding to the specified value string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public E this[string value]
        {
            get
            {
                foreach (E entry in _values.Values)
                {
                    if (entry.Value == value)
                        return entry;
                }
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns an array of the displayable values for this enum table, for use in presentation layer.
        /// </summary>
        public string[] Values
        {
            get
            {
                if (_displayValues == null)
                {
                    _displayValues = new string[_values.Count];
                    int i = 0;
                    foreach (E entry in _values.Values)
                        _displayValues[i++] = entry.Value;
                }
                return _displayValues;
            }
        }
     }
}
