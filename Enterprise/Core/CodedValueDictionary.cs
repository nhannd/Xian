using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Utilities;
using System.IO;
using System.Reflection;
using Iesi.Collections;
using System.Collections;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Represents a set of coded-values
    /// </summary>
    /// <typeparam name="TCodedValue">The class of coded-value that is contained in this dictionary</typeparam>
    public class CodedValueDictionary<TCodedValue>
        where TCodedValue : CodedValue
    {
        private IList<TCodedValue> _values;

        public CodedValueDictionary(IList<TCodedValue> values)
        {
            _values = values;
        }

        /// <summary>
        /// Obtains the instance for the specified code, or throws an exception if the code is invalid
        /// </summary>
        /// <param name="code"></param>
        /// <exception cref="ArgumentOutOfRangeException">The code is not valid</exception>
        /// <returns></returns>
        public TCodedValue LookupByCode(string code)
        {
            TCodedValue instance = CollectionUtils.SelectFirst<TCodedValue>(_values, delegate(TCodedValue v) { return v.Code.Equals(code); });
            if (instance == null)
                throw new ArgumentOutOfRangeException(string.Format("{0} is not a valid code", code));
            return instance;
        }

        /// <summary>
        /// Obtains the instance for the specified value, or throws an exception if the code is invalid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TCodedValue LookupByValue(string value)
        {
            TCodedValue instance = CollectionUtils.SelectFirst<TCodedValue>(_values, delegate(TCodedValue v) { return v.Value == value; });
            if (instance == null)
                throw new ArgumentOutOfRangeException(string.Format("{0} is not a valid value", value));
            return instance;
        }
        
        /// <summary>
        /// Returns the set of values that are active
        /// </summary>
        public TCodedValue[] ActiveValues
        {
            get { return GetValues(true); } 
        }

        /// <summary>
        /// Returns the entire set of values
        /// </summary>
        public TCodedValue[] Values
        {
            get { return GetValues(false); }
        }

        /// <summary>
        /// Returns the set of display strings for values that are active
        /// </summary>
        public string[] ActiveDisplayValues
        {
            get { return GetDisplayValues(true); }
        }

        /// <summary>
        /// Returns the entire set of display values
        /// </summary>
        public string[] DisplayValues
        {
            get { return GetDisplayValues(false); }
        }

        #region Helper methods

        private string[] GetDisplayValues(bool activeOnly)
        {
            return CollectionUtils.ToArray<string>(
                CollectionUtils.Map<TCodedValue, string>(SelectValues(activeOnly),
                    delegate(TCodedValue value) { return value.Value; }));
        }

        private TCodedValue[] GetValues(bool activeOnly)
        {
            return CollectionUtils.ToArray<TCodedValue>(SelectValues(activeOnly));
        }

        private IEnumerable SelectValues(bool activeOnly)
        {
            return activeOnly ? (IEnumerable)CollectionUtils.Select(_values, delegate(object obj) { return (obj as TCodedValue).Active; }) :
                (IEnumerable)_values;
        }

        #endregion
    }
}
