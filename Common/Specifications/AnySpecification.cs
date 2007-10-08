using System;
using System.Collections;
using System.Text;
using ClearCanvas.Common.Utilities;

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
