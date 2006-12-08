using System;
using System.Collections;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    public class EachSpecification : EnumerableSpecification
    {
        public EachSpecification(string testExpression, ISpecification elementSpecification, string failureMessage)
            : base(testExpression, elementSpecification, failureMessage)
        {
        }

        protected override TestResult InnerTest(object exp)
        {
            foreach (object element in AsEnumerable(exp))
            {
                TestResult r = this.ElementSpec.Test(element);
                if (r.Fail)
                    return new TestResult(false, new TestResultReason(this.FailureMessage, r.Reason));
            }

            return new TestResult(true);
        }
   }
}
