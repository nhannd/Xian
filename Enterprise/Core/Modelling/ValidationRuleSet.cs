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
using System.Collections.Generic;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Encapsulates a set of validation rules.
	/// </summary>
	/// <remarks>
	/// Instances of this class are immutable.
	/// </remarks>
	public class ValidationRuleSet : IValidationRuleSet
	{
		class AlwaysTrue : ISpecification
		{
			public TestResult Test(object obj)
			{
				return new TestResult(true);
			}
		}


		private readonly List<ISpecification> _rules;
		private readonly ISpecification _applicabilityRule;

		/// <summary>
		/// Constructor
		/// </summary>
		public ValidationRuleSet()
			: this(new List<ISpecification>(), new AlwaysTrue())
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rules"></param>
		public ValidationRuleSet(IEnumerable<ISpecification> rules)
			: this(rules, new AlwaysTrue())
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rules"></param>
		/// <param name="applicabilityRule"></param>
		public ValidationRuleSet(IEnumerable<ISpecification> rules, ISpecification applicabilityRule)
		{
			_rules = new List<ISpecification>(rules);
			_applicabilityRule = applicabilityRule;
		}

		/// <summary>
		/// Returns a new rule set representing the addition of this rule set and the other.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public ValidationRuleSet Add(ValidationRuleSet other)
		{
			return new ValidationRuleSet(new[] { this, other });
		}

		/// <summary>
		/// Returns a new rule set representing the addition of the specified rule sets.
		/// </summary>
		/// <param name="ruleSets"></param>
		/// <returns></returns>
		public static ValidationRuleSet Add(IList<ValidationRuleSet> ruleSets)
		{
			return new ValidationRuleSet(CollectionUtils.Map<ValidationRuleSet, ISpecification>(ruleSets, r => r));
		}

		#region ISpecification Members

		/// <summary>
		/// Tests all rules against the specified object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public TestResult Test(object obj)
		{
			return TestCore(obj, null);
		}

		#endregion

		#region IValidationRuleSet Members

		/// <summary>
		/// Tests the subset of rules (those that are selected by the filter) against the specified object.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public TestResult Test(object obj, Predicate<ISpecification> filter)
		{
			return TestCore(obj, filter);
		}

		#endregion

		/// <summary>
		/// Gets the list of rules contained in this rule set.
		/// </summary>
		internal IList<ISpecification> Rules
		{
			get { return _rules.AsReadOnly(); }
		}

		/// <summary>
		/// Gets the specification that indicates whether this rule-set is applicable to a given test object.
		/// </summary>
		internal ISpecification ApplicabilityRule
		{
			get { return _applicabilityRule; }
		}

		private TestResult TestCore(object obj, Predicate<ISpecification> filter)
		{
			Platform.CheckForNullReference(obj, "obj");

			// test applicability of this rule set - if it fails, this rule set is not applicable, hence the result of testing it is success
			if (_applicabilityRule.Test(obj).Fail)
				return new TestResult(true);

			// test every specification in the set of rules
			var failureReasons = new List<TestResultReason>();
			foreach (var rule in _rules)
			{
				// if there is no filter, or the fitler accepts the rule, test it
				if (filter == null || filter(rule))
				{
					// if the rule is itself a ruleset, then apply the filter recursively
					var result = (rule is IValidationRuleSet) ? (rule as IValidationRuleSet).Test(obj, filter) : rule.Test(obj);
					if (result.Fail)
					{
						failureReasons.AddRange(result.Reasons);
					}
				}
			}

			return new TestResult(failureReasons.Count == 0, failureReasons.ToArray());
		}
	}
}
