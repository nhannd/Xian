using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public interface IValidationRuleSet
    {
        void Add(IValidationRule rule);
        void Remove(IValidationRule rule);
        List<ValidationResult> GetResults();
        List<ValidationResult> GetResults(string propertyName);
    }
}
