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
    public class IsNullSpecification : PrimitiveSpecification
    {
        public IsNullSpecification()
        {

        }

        protected override TestResult InnerTest(object exp, object root)
        {
            // treat string "" as null, in the case where exp is a string
            return DefaultTestResult(exp == null || ((exp as string) == ""));
        }
    }
}
