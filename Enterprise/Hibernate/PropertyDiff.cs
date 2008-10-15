using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using NHibernate.Type;
using ClearCanvas.Common.Utilities;
using NHibernate.Collection;

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

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="hibernateType"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
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
				// if we're dealing with a collection property that has both old and new values,
				// need to compare the collection elements
				if(IsCollectionProperty && _oldValue != null && _newValue != null)
				{
					//TODO: collections with list semantics should use order-sensitive comparisons, but how do we know??
					//(e.g how do we differentiate a "bag" from a "list"?)
					return !CollectionUtils.Equal((ICollection) _oldValue, (ICollection) _newValue, false);
				}

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
