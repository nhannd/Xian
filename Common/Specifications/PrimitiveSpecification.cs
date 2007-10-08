using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public abstract class PrimitiveSpecification : Specification
    {
        public PrimitiveSpecification()
        {

        }

        protected TestResult DefaultTestResult(bool success)
        {
            return new TestResult(success, success ? null : new TestResultReason(this.FailureMessage));
        }
    }
}
