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
    public class DefinedSpecification : Specification
    {
        private ISpecification _lambda;

        public DefinedSpecification(ISpecification lambda)
        {
            _lambda = lambda;
        }

        protected override TestResult InnerTest(object exp, object root)
        {
            TestResult r = _lambda.Test(exp);
            if (r.Success)
            {
                return new TestResult(true);
            }
            else
            {
                return new TestResult(false, new TestResultReason(this.FailureMessage, r.Reasons));
            }
        }
    }
}
