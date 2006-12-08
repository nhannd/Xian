using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public abstract class PrimitiveSpecification : Specification
    {
        public PrimitiveSpecification(string testExpression, string failureMessage)
            :base(testExpression, failureMessage)
        {
        }

        public override IEnumerable<ISpecification> SubSpecs
        {
            get { return new ISpecification[] { }; }
        }

        protected TestResult DefaultTestResult(bool success)
        {
            return new TestResult(success, success ? null : new TestResultReason(this.FailureMessage));
        }
    }
}
