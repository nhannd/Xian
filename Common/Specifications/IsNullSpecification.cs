using System;
using System.Collections.Generic;
using System.Text;

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
