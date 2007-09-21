using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public abstract class SimpleInvariantSpecification : ISpecification, IPropertyBoundRule
    {
        private PropertyInfo[] _properties;

        public SimpleInvariantSpecification(PropertyInfo[] properties)
        {
            _properties = properties;
        }

        public SimpleInvariantSpecification(PropertyInfo property)
        {
            _properties = new PropertyInfo[] { property };
        }


        #region ISpecification Members

        public abstract TestResult Test(object obj);

        #endregion


        public PropertyInfo[] Properties
        {
            get { return _properties; }
        }

        public PropertyInfo Property
        {
            get { return _properties[0]; }
        }

        protected object GetPropertyValue(object obj)
        {
            
            return GetPropertyValue(obj, _properties[0]);
        }

        protected object[] GetPropertyValues(object obj)
        {
            return CollectionUtils.Map<PropertyInfo, object>(_properties,
                delegate(PropertyInfo property) { return GetPropertyValue(obj, property); }).ToArray();
        }

        private object GetPropertyValue(object obj, PropertyInfo property)
        {
            return property.GetGetMethod().Invoke(obj, null);
        }
    }
}
