using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common;
using System.Collections.ObjectModel;
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Encapsulates a set of 
    /// Note: immutable
    /// </summary>
    public class ValidationRuleSet : IValidationRuleSet
    {
        private List<ISpecification> _rules;

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationRuleSet()
        {
            _rules = new List<ISpecification>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rules"></param>
        public ValidationRuleSet(IEnumerable<ISpecification> rules)
        {
            _rules = new List<ISpecification>(rules);
        }

        /// <summary>
        /// Returns a new instance that contains both the rules contained in this set and
        /// in the other set.  Does not modify this instance.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ValidationRuleSet Combine(ValidationRuleSet other)
        {
            List<ISpecification> combined = new List<ISpecification>();
            combined.AddRange(this._rules);
            combined.AddRange(other._rules);

            return new ValidationRuleSet(combined);
        }

        /// <summary>
        /// Provides read-only access to the list of rules contained in this rule set.
        /// </summary>
        public IList<ISpecification> Rules
        {
            get { return _rules.AsReadOnly(); }
        }

        #region ISpecification Members

        /// <summary>
        /// Tests all rules against the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public TestResult Test(object obj)
        {
            return TestCore(obj, null);
        }

        #endregion

        #region IValidationRuleSet Members

        /// <summary>
        /// Tests the subset of rules (those that are selected by the filter) against the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TestResult Test(object obj, Predicate<ISpecification> filter)
        {
            return TestCore(obj, filter);
        }

        #endregion

        public TestResult TestCore(object obj, Predicate<ISpecification> filter)
        {
            Platform.CheckForNullReference(obj, "obj");

            // test every specification in the set of rules
            List<TestResultReason> failureReasons = new List<TestResultReason>();
            foreach (ISpecification rule in _rules)
            {
                // if there is no filter, or the fitler accepts the rule, test it
                if (filter == null || filter(rule))
                {
                    // if the rule is itself a ruleset, then apply the filter recursively
                    TestResult result = (rule is IValidationRuleSet) ? (rule as IValidationRuleSet).Test(obj, filter) : rule.Test(obj);
                    if (result.Fail)
                    {
                        failureReasons.AddRange(result.Reasons);
                    }
                }
            }

            return new TestResult(failureReasons.Count == 0, failureReasons.ToArray());
        }
    }
}
