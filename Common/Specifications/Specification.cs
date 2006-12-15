using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Scripting;

namespace ClearCanvas.Common.Specifications
{
    public abstract class Specification : ISpecification
    {
        private Expression _testExpr = Expression.Null;
        private Expression _ifExpr = Expression.Null;

        private string _failureMessage;

        public Specification()
        {
        }

        public Specification(string testExpression, string failureMessage)
        {
            _testExpr = new Expression(testExpression);
            _failureMessage = failureMessage;
        }

        public string TestExpression
        {
            get { return _testExpr.Text; }
            set { _testExpr = new Expression(value); }
        }

        public string IfExpression
        {
            get { return _ifExpr == null ? null : _ifExpr.Text; }
            set { _ifExpr = new Expression(value); }
        }

        public string FailureMessage
        {
            get { return _failureMessage; }
            set { _failureMessage = value; }
        }

        #region ISpecification Members

        public abstract IEnumerable<ISpecification> SubSpecs { get; }

        public TestResult Test(object obj)
        {
            bool doTest = true;
            if (!_ifExpr.Equals(Expression.Null))
            {
                object ifResult = _ifExpr.Evaluate(obj);
                if (ifResult is bool)
                    doTest = (bool)ifResult;
                else
                    throw new SpecificationException(SR.ExceptionCastExpressionBoolean);
            }

            if (doTest)
            {
                return InnerTest(_testExpr.Equals(Expression.Null) ? obj : _testExpr.Evaluate(obj));
            }
            else
            {
                return new TestResult(true);
            }
        }

        #endregion

        protected abstract TestResult InnerTest(object exp);
    }
}
