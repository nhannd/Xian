using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Holds a domain enumeration table in memory.
    /// 
    /// A domain enumeration is an enumeration, or set of coded values, that is part of the domain model.
    /// In order to create a domain enumeration, a C# enum struct must first be defined based on the codes
    /// in the enumeration.  Additional data about the enumeration, such as the values corresponding to the codes,
    /// and descriptions of the meanings of those values, is typically stored in persistent storage.
    /// 
    /// An instance of <see cref="IEnumBroker"/> is used to load the <see cref="EnumTable"/>.
    /// See <cref="IEnumBroker.LoadEnumeration"/>.
    /// </summary>
    /// <typeparam name="TEnum">The C# enum that this table corresponds to</typeparam>
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
