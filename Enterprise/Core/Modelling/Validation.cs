using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public static class Validation
    {
        private static Dictionary<Type, ValidationRuleSet> _invariantRuleSets = new Dictionary<Type, ValidationRuleSet>();

        public static ValidationRuleSet GetInvariantRules(object obj)
        {
            Type type = obj.GetType();
            lock (_invariantRuleSets)
            {
                if (_invariantRuleSets.ContainsKey(type))
                {
                    return _invariantRuleSets[type];
                }
                else
                {
                    ValidationBuilder builder = new ValidationBuilder();
                    ValidationRuleSet rules = builder.BuildRuleSet(type);
                    _invariantRuleSets[type] = rules;
                    return rules;
                }
            }
        }
    }
}
