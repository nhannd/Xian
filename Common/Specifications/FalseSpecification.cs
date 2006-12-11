using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class FalseSpecification : PrimitiveSpecification
    {
        public FalseSpecification(string testExpr, string failureMessage)
            : base(testExpr, failureMessage)
        {
        }

        protected override TestResult InnerTest(object exp)
        {
            if (exp is bool)
            {
                return DefaultTestResult(!(bool)exp);
            }
            else
            {
                throw new SpecificationException("Expression must evaluate to Boolean type");
            }
        }
    }
}
