#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
	internal class EmbeddedValueRuleSet : IValidationRuleSet, IPropertyBoundRule
	{
		private readonly ValidationRuleSet _innerRules;
		private readonly PropertyInfo _property;
		private readonly bool _collection;

		public EmbeddedValueRuleSet(PropertyInfo property, ValidationRuleSet innerRules, bool collection)
		{
			_property = property;
			_innerRules = innerRules;
			_collection = collection;
		}

		#region ISpecification Members

		public TestResult Test(object obj)
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

		#region IPropertyBoundRule Members

		public PropertyInfo[] Properties
		{
			get { return new [] { _property }; }
		}

		#endregion

		protected TestResult TestCore(object obj, Predicate<ISpecification> filter)
		{
			var propertyValue = _property.GetGetMethod().Invoke(obj, null);

			// if the propertyValue is null, return true
			// this seems counter-intuitive, but what we are effectively saying is that the rules
			// are bound to the propertyValue being tested - if there is no propertyValue, there are no rules to test
			if (propertyValue == null)
				return new TestResult(true);

			if (_collection)
			{
				// apply to items rather than to the collection
				foreach (var item in (propertyValue as IEnumerable))
				{
					var result = _innerRules.Test(item, filter);
					// if any item fails, don't bother testing the rest of the items
					if (result.Fail)
					{
						var message = string.Format(SR.RuleEmbeddeValueCollection, TerminologyTranslator.Translate(_property));
						return new TestResult(false, new TestResultReason(message, result.Reasons));
					}
				}
				return new TestResult(true);
			}
			else
			{
				var result = _innerRules.Test(propertyValue, filter);
				if (result.Fail)
				{
					var message = string.Format(SR.RuleEmbeddeValue, TerminologyTranslator.Translate(_property));
					return new TestResult(false, new TestResultReason(message, result.Reasons));
				}
				return new TestResult(true);
			}
		}
	}
}
