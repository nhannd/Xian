#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Default implemenation of <see cref="IValidationRuleSet"/>.
    /// </summary>
    public class ValidationRuleSet : IValidationRuleSet
    {
        private List<IValidationRule> _rules;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidationRuleSet()
        {
            _rules = new List<IValidationRule>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rules"></param>
        public ValidationRuleSet(IList<IValidationRule> rules)
        {
            _rules = new List<IValidationRule>(rules);
        }

		/// <summary>
		/// Gets the concatenation of all error strings, based on the results of all
		/// <see cref="IValidationRule"/>s in the set.
		/// </summary>
		public string GetErrorsString(IApplicationComponent component)
		{
			List<IValidationRule> brokenRules = _rules.FindAll(
				delegate(IValidationRule r) { return r.GetResult(component).Success == false; });

			return StringUtilities.Combine(brokenRules, "\n",
				delegate(IValidationRule r)
				{
					return string.Format("{0}: {1}",
						r.PropertyName,
						StringUtilities.Combine(r.GetResult(component).Messages, ", "));
				});
		}

        #region IValidationRuleSet members

    	/// <summary>
    	/// Adds a rule to the set.
    	/// </summary>
    	public void Add(IValidationRule rule)
        {
            _rules.Add(rule);
        }

		/// <summary>
		/// Adds rules to the set.
		/// </summary>
		public void AddRange(IEnumerable<IValidationRule> rules)
		{
			_rules.AddRange(rules);
		}

    	/// <summary>
    	/// Removes a rule from the set.
    	/// </summary>
    	public void Remove(IValidationRule rule)
        {
            _rules.Remove(rule);
        }

    	/// <summary>
    	/// Evaluates every rule in the set against the specified component.
    	/// </summary>
    	/// <param name="component">Component to validate.</param>
    	public List<ValidationResult> GetResults(IApplicationComponent component)
        {
            return GetResults(component, _rules);
        }

    	/// <summary>
    	/// Evaluates all rules in the set that apply to the specified property against the specified component.
    	/// </summary>
    	/// <param name="component">Component to validate.</param>
    	/// <param name="propertyName">Property to validate.</param>
    	public List<ValidationResult> GetResults(IApplicationComponent component, string propertyName)
        {
            return GetResults(component, _rules.FindAll(delegate(IValidationRule v) { return v.PropertyName == propertyName; }));
        }

        #endregion

        private static List<ValidationResult> GetResults(IApplicationComponent component, List<IValidationRule> validators)
        {
            return validators.ConvertAll<ValidationResult>(delegate(IValidationRule v) { return v.GetResult(component); });
        }
    }
}
