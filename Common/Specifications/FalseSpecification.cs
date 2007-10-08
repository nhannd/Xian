using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class FalseSpecification : PrimitiveSpecification
    {
        public FalseSpecification()
        {

        }

        protected override TestResult InnerTest(object exp, object root)
        {
            if (exp is bool)
            {
                return DefaultTestResult(!(bool)exp);
            }
            else
            {
				throw new SpecificationException(SR.ExceptionCastExpressionBoolean);
            }
        }
    }
}
