using System;
using System.Collections;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    public class AnySpecification : EnumerableSpecification
    {
        public AnySpecification(string testExpression, ISpecification elementSpecification, string failureMessage)
            : base(testExpression, elementSpecification, failureMessage)
        {
        }

        protected override TestResult InnerTest(object exp)
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
