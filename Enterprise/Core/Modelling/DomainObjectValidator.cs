using System;
using System.Collections.Generic;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Provides domain object validation functionality.
	/// </summary>
	/// <remarks>
	/// Instances of this class are not thread-safe and should never be used by more than one thread.
	/// </remarks>
	public class DomainObjectValidator
	{
		private readonly Dictionary<Type, ValidationRuleSet> _ruleSets = new Dictionary<Type, ValidationRuleSet>();

		/// <summary>
		/// Validates the specified domain object.
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="EntityValidationException">Validation failed.</exception>
		public void Validate(DomainObject obj)
		{
			// validate all rules
			Validate(obj, rule => true);
		}

		/// <summary>
		/// Validates the specified domain object, ignoring any rules that do not satisfy the filter.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="ruleFilter"></param>
		/// <exception cref="EntityValidationException">Validation failed.</exception>
		public void Validate(DomainObject obj, Predicate<ISpecification> ruleFilter)
		{
			var domainClass = obj.GetClass();

			ValidationRuleSet rules;

			// first check for a cached rule-set
			if (!_ruleSets.TryGetValue(domainClass, out rules))
			{
				// otherwise build it
				rules = ValidationRuleSetCache.GetInvariantRules(domainClass)
					.Add(ValidationRuleSetCache.GetCustomRules(domainClass));
				_ruleSets.Add(domainClass, rules);
			}

			var result = rules.Test(obj, ruleFilter);
			if (result.Fail)
			{
				var message = string.Format(SR.ExceptionInvalidEntity, TerminologyTranslator.Translate(obj.GetClass()));
				throw new EntityValidationException(message, result.Reasons);
			}
		}
	}
}
