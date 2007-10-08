using System;
using System.Collections;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Specifications
{
    public class EachSpecification : EnumerableSpecification
    {
        public EachSpecification(ISpecification elementSpecification)
            :base(elementSpecification)
        {
        }

        protected override TestResult InnerTest(object exp, object root)
        {
            foreach (object element in AsEnumerable(exp))
            {
                TestResult r = this.ElementSpec.Test(element);
                if (r.Fail)
                    return new TestResult(false, new TestResultReason(this.FailureMessage, r.Reasons));
            }

            return new TestResult(true);
        }
   }
}
