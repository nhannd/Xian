#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
