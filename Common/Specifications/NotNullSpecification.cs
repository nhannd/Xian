using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class NotNullSpecification : PrimitiveSpecification
    {
        public NotNullSpecification()
        {

        }

        protected override TestResult InnerTest(object exp, object root)
        {
            // treat string "" as null, in the case where exp is a string
            return DefaultTestResult(exp != null && ((exp as string) != ""));
        }
    }
}
