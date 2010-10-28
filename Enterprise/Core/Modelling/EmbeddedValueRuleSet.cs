#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Reflection;
using ClearCanvas.Common.Specifications;
using System.Collections;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Similar to a <see cref="ValidationRuleSet"/>, this class encapsulates an instance of a <see cref="ValidationRuleSet"/>
	/// that is applied to an embedded value or embedded value collection of a parent object.  The child rule-set is
	/// evaluated only if the embedded value is non-null.
	/// </summary>
	internal class EmbeddedValueRuleSet : SimpleInvariantSpecification, IValidationRuleSet
	{
		private readonly ValidationRuleSet _innerRules;
		private readonly bool _collection;

		internal EmbeddedValueRuleSet(PropertyInfo property, ValidationRuleSet innerRules, bool collection)
			:base(property)
		{
			_innerRules = innerRules;
			_collection = collection;
		}

		#region ISpecification Members

		public override TestResult Test(object obj)
		{
			return TestCore(obj, spec => true);
		}

		#endregion

		#region IValidationRuleSet Members

		public TestResult Test(object obj, Predicate<ISpecification> filter)
		{
			return TestCore(obj, filter);
		}

		#endregion

		protected TestResult TestCore(object obj, Predicate<ISpecification> filter)
		{
			var propertyValue = GetPropertyValue(obj);

			// if the propertyValue is null, return true
			// this seems counter-intuitive, but what we are effectively saying is that the rules
			// are bound to the propertyValue being tested - if there is no propertyValue, there are no rules to test
			if (propertyValue == null)
				return new TestResult(true);

			return _collection ? TestCollection((IEnumerable)propertyValue, filter) : TestSingleValue(propertyValue, filter);
		}

		private TestResult TestSingleValue(object propertyValue, Predicate<ISpecification> filter)
		{
			var result = _innerRules.Test(propertyValue, filter);
			if (result.Fail)
			{
				var message = string.Format(SR.RuleEmbeddeValue, TerminologyTranslator.Translate(this.Property));
				return new TestResult(false, new TestResultReason(message, result.Reasons));
			}
			return new TestResult(true);
		}

		private TestResult TestCollection(IEnumerable collection, Predicate<ISpecification> filter)
		{
			foreach (var item in collection)
			{
				var result = _innerRules.Test(item, filter);
				// if any item fails, don't bother testing the rest of the items
				if (result.Fail)
				{
					var message = string.Format(SR.RuleEmbeddeValueCollection, TerminologyTranslator.Translate(this.Property));
					return new TestResult(false, new TestResultReason(message, result.Reasons));
				}
			}
			return new TestResult(true);
		}
	}
}
