using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Specifies that a given property of an object is unique within the set of persistent instances of that object's class.
    /// </summary>
    /// <remarks>
    /// This class is similar to <see cref="UniqueKeySpecification"/>, except that the unique key is limited to a single
    /// primitive-valued property.  However, this limitation allows this class to implement <see cref="IPropertyBoundRule"/>
    /// since the key value is a function of a single property.
    /// </remarks>
    public class UniqueSpecification : UniqueKeySpecification, IPropertyBoundRule
    {
        private PropertyInfo _property;

        public UniqueSpecification(PropertyInfo property)
            : base(property.Name, new string[] { property.Name })
        {
            _property = property;
        }

        #region IPropertyBoundRule Members

        public PropertyInfo[] Properties
        {
            get { return new PropertyInfo[] { _property }; }
        }

        #endregion
    }
}
