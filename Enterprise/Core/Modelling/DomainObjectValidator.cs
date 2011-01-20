#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Provides domain object validation functionality.
	/// </summary>
	/// <remarks>
	/// Instances of this class are not thread-safe and should never be used by more than one thread.
	/// A corollary is that these objects are intended to be short-lived, not spanning more than a
	/// single request.
	/// </remarks>
	public class DomainObjectValidator
	{
		private static readonly Type[] _lowLevelRuleClasses = new[]
		{
			typeof(LengthSpecification),
			typeof(RequiredSpecification),
			typeof(UniqueKeySpecification),
			typeof(UniqueSpecification),
			typeof(EmbeddedValueRuleSet)
		};

		private readonly Dictionary<Type, ValidationRuleSet> _ruleSets = new Dictionary<Type, ValidationRuleSet>();

		#region Public API

		/// <summary>
		/// Checks whether validation is enabled on the specified domain class.
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
		public static bool IsValidationEnabled(Type domainClass)
		{
			var a = AttributeUtils.GetAttribute<ValidationAttribute>(domainClass, true);

			// if no attribute present, then by default validation is enabled
			return (a == null) || a.EnableValidation;
		}

		/// <summary>
		/// Validates the specified domain object, applying all known validation rules.
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="EntityValidationException">Validation failed.</exception>
		public void Validate(DomainObject obj)
		{
			// validate all rules
			Validate(obj, rule => true);
		}

		/// <summary>
		/// Validates only that the specified object has required fields set.
		/// </summary>
		/// <param name="obj"></param>
		public void ValidateRequiredFieldsPresent(DomainObject obj)
		{
			ValidateLowLevel(obj, rule => rule is RequiredSpecification);
		}


		/// <summary>
		/// Validates the specified domain object, applying only "low-level" rules, subject to the specified filter.
		/// </summary>
		/// <remarks>
		/// Low-level rules are:
		/// 1. Required fields.
		/// 2. String field lengths.
		/// 3. Unique constraints.
		/// </remarks>
		/// <param name="obj"></param>
		/// <param name="ruleFilter"></param>
		public void ValidateLowLevel(DomainObject obj, Predicate<ISpecification> ruleFilter)
		{
			// construct a predicate which says:
			// 1. if the rule is a low-level rule class, let the caller's ruleFilter decide
			// 2. if the rule is a rule-set (but not an embedded-value ruleset which has already been covered in 1), then evaluate it
			Validate(obj, rule => IsLowLevelRule(rule)? ruleFilter(rule) : rule is IValidationRuleSet);
		}

		/// <summary>
		/// Validates the specified domain object, applying only high-level rules.
		/// </summary>
		/// <remarks>
		/// High-level rules include any rules that are not low-level rules.
		/// </remarks>
		/// <param name="obj"></param>
		public void ValidateHighLevel(DomainObject obj)
		{
			Validate(obj, r => !IsLowLevelRule(r));
		}

		#endregion

		/// <summary>
		/// Validates the specified domain object, ignoring any rules that do not satisfy the filter.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="ruleFilter"></param>
		/// <exception cref="EntityValidationException">Validation failed.</exception>
		private void Validate(DomainObject obj, Predicate<ISpecification> ruleFilter)
		{
			var domainClass = obj.GetClass();

			ValidationRuleSet rules;

			// first check for a cached rule-set
			if (!_ruleSets.TryGetValue(domainClass, out rules))
			{
				// otherwise build it
				rules = IsValidationEnabled(domainClass) ?
					ValidationRuleSetCache.GetInvariantRules(domainClass).Add(ValidationRuleSetCache.GetCustomRules(domainClass))
					: new ValidationRuleSet();

				_ruleSets.Add(domainClass, rules);
			}

			var result = rules.Test(obj, ruleFilter);
			if (result.Fail)
			{
				var message = string.Format(SR.ExceptionInvalidEntity, TerminologyTranslator.Translate(obj.GetClass()));
				throw new EntityValidationException(message, result.Reasons);
			}
		}

		/// <summary>
		/// Checks if the specified rule is considered a low-level rule.
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		private static bool IsLowLevelRule(ISpecification rule)
		{
			return CollectionUtils.Contains(_lowLevelRuleClasses, t => t.Equals(rule.GetType()));
		}
	}
}
