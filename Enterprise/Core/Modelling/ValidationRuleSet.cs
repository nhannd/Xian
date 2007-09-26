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
    /// Note: immutable
    /// </summary>
    public class ValidationRuleSet : IValidationRuleSet
    {
        private List<ISpecification> _rules;

        public ValidationRuleSet()
        {
            _rules = new List<ISpecification>();
        }

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

        public IList<ISpecification> Rules
        {
            get { return _rules.AsReadOnly(); }
        }

        #region ISpecification Members

        public TestResult Test(object obj)
        {
            return TestCore(obj, null);
        }

        #endregion

        #region IValidationRuleSet Members

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
