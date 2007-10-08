using System;
using System.Collections.Generic;
using System.Text;

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
