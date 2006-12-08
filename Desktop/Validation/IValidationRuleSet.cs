using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Defines the interface to a set of <see cref="IValidationRule"/> objects.
    /// </summary>
    public interface IValidationRuleSet
    {
        /// <summary>
        /// Adds a rule to the set
        /// </summary>
        /// <param name="rule"></param>
        void Add(IValidationRule rule);

        /// <summary>
        /// Removes a rule from the set
        /// </summary>
        /// <param name="rule"></param>
        void Remove(IValidationRule rule);

        /// <summary>
        /// Evaluates every rule in the set and obtains the results.
        /// </summary>
        /// <returns></returns>
        List<ValidationResult> GetResults();

        /// <summary>
        /// Evaluates all rules in the set that apply to the specified property, and returns the results.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        List<ValidationResult> GetResults(string propertyName);
    }
}
