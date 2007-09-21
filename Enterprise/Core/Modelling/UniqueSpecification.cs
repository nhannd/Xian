using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public class UniqueSpecification : UniqueKeySpecification, IPropertyBoundRule
    {
        private PropertyInfo _property;

        public UniqueSpecification(PropertyInfo property)
            : base(new string[] { property.Name }, property.Name)
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
