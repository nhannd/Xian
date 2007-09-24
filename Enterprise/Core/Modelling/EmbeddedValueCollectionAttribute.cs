using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EmbeddedValueCollectionAttribute : Attribute
    {
        private Type _elementType;

        public EmbeddedValueCollectionAttribute(Type elementType)
        {
            _elementType = elementType;
        }

        public Type ElementType
        {
            get { return _elementType; }
        }
    }
}
