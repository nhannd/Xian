#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System.Text.RegularExpressions;

namespace ClearCanvas.Common.Specifications
{
    public class RegexSpecification : PrimitiveSpecification
    {
        private readonly string _pattern;
    	private readonly bool _ignoreCase = true;	// true by default
    	private readonly bool _nullMatches;

        public RegexSpecification(string pattern, bool ignoreCase, bool nullMatches)
        {
			Platform.CheckForNullReference(pattern, "pattern");
			Platform.CheckForEmptyString(pattern, "pattern");

            _pattern = pattern;
            _ignoreCase = ignoreCase;
        	_nullMatches = nullMatches;
        }

		public RegexSpecification(string pattern)
			:this(pattern, true, false)
		{
		}

    	public string Pattern
    	{
			get { return _pattern; }
    	}

    	public bool IgnoreCase
    	{
			get { return _ignoreCase; }
    	}

    	public bool NullMatches
    	{
			get { return _nullMatches; }
    	}

        protected override TestResult InnerTest(object exp, object root)
        {
            if (exp == null)
				return DefaultTestResult(_nullMatches);

            if (exp is string)
            {
                if (_ignoreCase)
                    return DefaultTestResult(Regex.Match(exp as string, _pattern, RegexOptions.IgnoreCase).Success);
                else
                    return DefaultTestResult(Regex.Match(exp as string, _pattern).Success);
            }
            else
            {
                throw new SpecificationException(SR.ExceptionCastExpressionString);
            }
        }
    }
}
