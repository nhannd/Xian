using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public abstract class ComparisonSpecification : PrimitiveSpecification
    {
        private Expression _refValueExpr;

        public Expression RefValueExpression
        {
            get { return _refValueExpr; }
            set { _refValueExpr = value; }
        }

        protected override TestResult InnerTest(object exp, object root)
        {
            if (_refValueExpr == null)
                throw new SpecificationException("Reference value required.");

            object refValue = _refValueExpr.Evaluate(root);

            return DefaultTestResult(CompareValues(exp, refValue));
        }

        protected abstract bool CompareValues(object testValue, object refValue);
    }
}
