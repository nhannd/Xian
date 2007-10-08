using System;
using System.Collections.Generic;

namespace ClearCanvas.Common.Specifications
{
    public abstract class Specification : ISpecification
    {
        private Expression _testExpr = Expression.Null;

        private string _failureMessage;

        public Specification()
        {
        }

        public Expression TestExpression
        {
            get { return _testExpr; }
            set { _testExpr = value; }
        }

        public string FailureMessage
        {
            get { return _failureMessage; }
            set { _failureMessage = value; }
        }

        #region ISpecification Members

        public TestResult Test(object obj)
        {
            return InnerTest(_testExpr.Equals(Expression.Null) ? obj : _testExpr.Evaluate(obj), obj);
        }

        #endregion

        protected abstract TestResult InnerTest(object exp, object root);
    }
}
