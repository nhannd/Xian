using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using NHibernate.Type;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Used by <see cref="ChangeRecord"/> to record changes to individual properties.
    /// </summary>
    class PropertyDiff
    {
        private readonly string _propertyName;
        private readonly object _oldValue;
        private readonly object _newValue;
        private readonly IType _hibernateType;

        public PropertyDiff(string propertyName, IType hibernateType, object oldValue, object newValue)
        {
            _propertyName = propertyName;
            _hibernateType = hibernateType;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public bool IsCollectionProperty
        {
            get { return _hibernateType.IsCollectionType; }
        }

        public bool IsChanged
        {
            get
            {
                // check if the property is dirty
                // note: if the property is a collection, this comparison probably won't be accurate
                return !Equals(_oldValue, _newValue);
            }
        }

        public PropertyChange AsPropertyChange()
        {
            return new PropertyChange(_propertyName, _oldValue, _newValue);
        }

        /// <summary>
        /// Returns a new <see cref="PropertyDiff"/> that is the result of adding this change
        /// to <paramref name="previousChange"/>.
        /// </summary>
        /// <param name="previousChange"></param>
        /// <returns></returns>
        /// <remarks>
        /// This operation is not commutative.
        /// </remarks>
        public PropertyDiff Compound(PropertyDiff previousChange)
        {
            return new PropertyDiff(_propertyName, _hibernateType, previousChange._oldValue, _newValue);
        }
    }
}
