#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

namespace ClearCanvas.Common.Specifications
{
    public class TrueSpecification : PrimitiveSpecification
    {
        public TrueSpecification()
        {

        }

        protected override TestResult InnerTest(object exp, object root)
        {
            if (exp is bool)
            {
                return DefaultTestResult((bool)exp);
            }
            else
            {
                throw new SpecificationException(SR.ExceptionCastExpressionBoolean);
            }
        }
    }
}
