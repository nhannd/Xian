using System;
using System.Collections.Generic;
using System.Text;

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
    public class EnumTable<e, E> : IEnumerable<E>
        where e : struct
        where E : EnumValue<e>
    {
        private IDictionary<e, E> _values;

        /// <summary>
        /// Not used by client code.
        /// </summary>
        /// <param name="values"></param>
        public EnumTable(IDictionary<e, E> values)
        {
            _values = values;
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

        #region IEnumerable<E> Members

        public IEnumerator<E> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        #endregion
    }
}
