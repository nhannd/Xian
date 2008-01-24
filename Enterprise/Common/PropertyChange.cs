using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Represents a change made to a property value of an entity.
    /// </summary>
    public class PropertyChange
    {
        private readonly string _propertyName;
        private readonly object _oldValue;
        private readonly object _newValue;

        public PropertyChange(string propertyName, object oldValue, object newValue)
        {
            _propertyName = propertyName;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public object OldValue
        {
            get { return _oldValue; }
        }

        public object NewValue
        {
            get { return _newValue; }
        }

        /// <summary>
        /// Returns a new <see cref="PropertyChange"/> that is the result of adding this change
        /// to <paramref name="previousChange"/>.
        /// </summary>
        /// <param name="previousChange"></param>
        /// <returns></returns>
        /// <remarks>
        /// This operation is not commutative.
        /// </remarks>
        public PropertyChange AddTo(PropertyChange previousChange)
        {
            return new PropertyChange(_propertyName, previousChange._oldValue, _newValue);
        }
    }
}
