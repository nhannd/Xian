using System;
using System.Collections;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class CountSpecification : PrimitiveSpecification
    {
        private int _min = 0;
        private int _max = Int32.MaxValue;

        public CountSpecification(string testExpression, int min, int max, string failureMessage)
            :base(testExpression, failureMessage)
        {
            _max = max;
            _min = min;
        }

        protected override TestResult InnerTest(object exp)
        {
            if (exp is Array)
            {
                return DefaultTestResult(InRange((exp as Array).Length));
            }

            if (exp is ICollection)
            {
                return DefaultTestResult(InRange((exp as ICollection).Count));
            }

            if (exp is IEnumerable)
            {
                // manually count the items
                // this could be very bad in terms of performance, but let's assume this will rarely
                // happen on a very large collection
                int count = 0;
                foreach (object element in (exp as IEnumerable)) count++;
                return DefaultTestResult(InRange(count));
            }

			throw new SpecificationException(SR.ExceptionCastExpressionArrayCollectionEnumerable);
        }

        protected bool InRange(int n)
        {
            return n >= _min && n <= _max;
        }
    }
}
