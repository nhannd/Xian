using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public static class Validation
    {
        private static Dictionary<Type, ValidationRuleSet> _invariantRuleSets = new Dictionary<Type, ValidationRuleSet>();

        public static ValidationRuleSet GetInvariantRules(Type entityClass)
        {
            lock (_invariantRuleSets)
            {
                ValidationRuleSet rules;

                // return cached rules if possible
                if (_invariantRuleSets.TryGetValue(entityClass, out rules))
                    return rules;

                // build rules for entityClass, and put in cache
                ValidationBuilder builder = new ValidationBuilder();
                rules = builder.BuildRuleSet(entityClass);
                _invariantRuleSets.Add(entityClass, rules);
                return rules;
            }
        }

        public static ValidationRuleSet GetInvariantRules(DomainObject obj)
        {
            return GetInvariantRules(obj.GetType());
        }

        public static void Validate(DomainObject obj, Predicate<ISpecification> ruleFilter)
        {
            ValidationRuleSet rules = Validation.GetInvariantRules(obj);

            TestResult result = rules.Test(obj, ruleFilter);
            if (result.Fail)
            {
                string message = string.Format(SR.ExceptionInvalidEntity, TerminologyTranslator.Translate(obj.GetType()));
                throw new EntityValidationException(message, result.Reasons);
            }
        }

        public static void Validate(DomainObject obj)
        {
            Validate(obj, null);
        }
    }
}
