using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// When applied to a collection-type property of a domain class, indicates that that property models
    /// an embedded collection of values as opposed to an association.
    /// </summary>
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
