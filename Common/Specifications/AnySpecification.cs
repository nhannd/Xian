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
    public class AnySpecification : EnumerableSpecification
    {
        public AnySpecification(ISpecification elementSpecification)
            :base(elementSpecification)
        {

        }

        protected override TestResult InnerTest(object exp, object root)
        {
            foreach (object element in AsEnumerable(exp))
            {
                TestResult r = this.ElementSpec.Test(element);
                if (r.Success)
                    return new TestResult(true);
            }

            // note that we can only return the immediate reason - there is no sensible way to return sub-reasons 
            return new TestResult(false, new TestResultReason(this.FailureMessage));
        }
    }
}
