#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System;
namespace ClearCanvas.Common.Specifications
{
    public abstract class ComparisonSpecification : PrimitiveSpecification
    {
        private Expression _refValueExpr;
    	private bool _strict;

        public Expression RefValueExpression
        {
            get { return _refValueExpr; }
            set { _refValueExpr = value; }
        }

    	public bool Strict
    	{
			get { return _strict; }
			set { _strict = value; }
    	}

        protected override TestResult InnerTest(object exp, object root)
        {
            if (_refValueExpr == null)
                throw new SpecificationException("Reference value required.");

            object refValue = _refValueExpr.Evaluate(root);

			// bug #3279:if the refValue is of a different type than the test expression, 
			// attempt to coerce the refValue to the same type
			// if the coercion fails, the comparison will be performed on the raw refValue
			if(!_strict && exp != null && refValue != null && exp.GetType() != refValue.GetType())
			{
				// try to coerce the reference value to the expression type
				var success = TryCoerce(ref refValue, exp.GetType());

				// bug #5909: if that didn't work, try coercing the expression value to the reference value type
				if(!success)
				{
					TryCoerce(ref exp, refValue.GetType());
				}

				// if neither of the above worked, then we just proceed to compare the raw values
			}

            return DefaultTestResult(CompareValues(exp, refValue));
        }

        protected abstract bool CompareValues(object testValue, object refValue);

		private static bool TryCoerce(ref object value, Type type)
		{
			try
			{
				value = Convert.ChangeType(value, type);
				return true;
			}
			catch (InvalidCastException)
			{
				// unable to cast - "value" is not modified
				return false;
			}
		}
    }
}
