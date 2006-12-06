using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    public class ValidationRuleSet : IValidationRuleSet
    {
        private List<IValidationRule> _rules;

        public ValidationRuleSet()
        {
            _rules = new List<IValidationRule>();
        }

        public void Add(IValidationRule rule)
        {
            _rules.Add(rule);
        }

        public void Remove(IValidationRule rule)
        {
            _rules.Remove(rule);
        }

        public List<ValidationResult> GetResults()
        {
            return GetResults(_rules);
        }

        public List<ValidationResult> GetResults(string propertyName)
        {
            return GetResults(_rules.FindAll(delegate(IValidationRule v) { return v.PropertyName == propertyName; }));
        }

        private List<ValidationResult> GetResults(List<IValidationRule> validators)
        {
            return validators.ConvertAll<ValidationResult>(delegate(IValidationRule v) { return v.Result; });
        }
    }
}
