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
        /// Evaluates every rule in the set against the specified component.
        /// </summary>
        /// <param name="component">Component to validate</param>
        /// <returns></returns>
        List<ValidationResult> GetResults(IApplicationComponent component);

        /// <summary>
        /// Evaluates all rules in the set that apply to the specified property against the specified component.
        /// </summary>
        /// <param name="component">Component to validate</param>
        /// <param name="propertyName">Property to validate</param>
        /// <returns></returns>
        List<ValidationResult> GetResults(IApplicationComponent component, string propertyName);
    }
}
