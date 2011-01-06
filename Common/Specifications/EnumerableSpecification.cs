#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System.Collections;

namespace ClearCanvas.Common.Specifications
{
    public abstract class EnumerableSpecification : Specification
    {
        private readonly ISpecification _elementSpecification;

        public EnumerableSpecification(ISpecification elementSpecification)
        {
			Platform.CheckForNullReference(elementSpecification, "elementSpecification");
            _elementSpecification = elementSpecification;
        }

        protected internal ISpecification ElementSpec
        {
            get { return _elementSpecification; }
        }

        protected static IEnumerable AsEnumerable(object obj)
        {
            IEnumerable enumerable = obj as IEnumerable;
            if (enumerable == null)
				throw new SpecificationException(SR.ExceptionCastExpressionEnumerable);

            return enumerable;
        }
    }
}
