using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class RequiredValueSpecification : PrimitiveSpecification
    {

        private object _nullValue;

        public RequiredValueSpecification(string testExpression, object nullValue, string failureMessage)
            :base(testExpression, failureMessage)
        {
            _nullValue = nullValue;
        }

        protected override TestResult InnerTest(object exp)
        {
            // if a null value was supplied, just test exp against it
            if (_nullValue != null)
            {
                return DefaultTestResult(!_nullValue.Equals(exp));
            }
            else
            {
                // _nullValue == null (the reference value is "null" itself)

                // make a special case for string - treat "" as null
                if (exp is string)
                {
                    return DefaultTestResult(exp != null && (exp as string) != "");
                }

                return DefaultTestResult(exp != null);
            }
        }
    }
}
