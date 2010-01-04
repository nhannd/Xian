#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
