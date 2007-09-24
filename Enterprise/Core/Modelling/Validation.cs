using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public static class Validation
    {
        private static Dictionary<Type, ValidationRuleSet> _invariantRuleSets = new Dictionary<Type, ValidationRuleSet>();

        public static ValidationRuleSet GetInvariantRules(Type entityClass)
        {
            lock (_invariantRuleSets)
            {
                if (_invariantRuleSets.ContainsKey(entityClass))
                {
                    return _invariantRuleSets[entityClass];
                }
                else
                {
                    ValidationBuilder builder = new ValidationBuilder();
                    ValidationRuleSet rules = builder.BuildRuleSet(entityClass);
                    _invariantRuleSets[entityClass] = rules;
                    return rules;
                }
            }
        }

        public static ValidationRuleSet GetInvariantRules(DomainObject obj)
        {
            return GetInvariantRules(obj.GetType());
        }
    }
}
