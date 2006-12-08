using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearCanvas.Common.Specifications
{
    public class RegexSpecification : PrimitiveSpecification
    {
        private string _pattern;

        public RegexSpecification(string testExpression, string pattern, string failureMessage)
            :base(testExpression, failureMessage)
        {
            _pattern = pattern;
        }

        protected override TestResult InnerTest(object exp)
        {
            // assume that null matches anything
            if (exp == null)
                return DefaultTestResult(true);

            if (exp is string)
            {
                return DefaultTestResult(Regex.Match(exp as string, _pattern).Success);
            }
            else
            {
                throw new SpecificationException("Expression must evaluate to a String type");
            }
        }
    }
}
