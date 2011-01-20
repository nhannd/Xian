#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

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
