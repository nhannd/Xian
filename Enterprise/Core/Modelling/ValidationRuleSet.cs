using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common;
using System.Collections.ObjectModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Note: immutable
    /// </summary>
    public class ValidationRuleSet : ISpecification
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
            return Test(obj, null);
        }

        #endregion

        public TestResult Test(object obj, List<string> dirtyProperties)
        {
            Platform.CheckForNullReference(obj, "obj");

            // test every specification in the set of rules
            List<TestResultReason> failureReasons = new List<TestResultReason>();
            foreach (ISpecification rule in _rules)
            {
                if (dirtyProperties == null || ShouldCheckRule(rule, dirtyProperties))
                {
                    TestResult result = rule.Test(obj);
                    if (result.Fail)
                    {
                        failureReasons.AddRange(result.Reasons);
                    }
                }
            }

            return new TestResult(failureReasons.Count == 0, failureReasons.ToArray());
        }

        private bool ShouldCheckRule(ISpecification rule, List<string> dirtyProperties)
        {
            // if the rule is not bound to specific properties, then it should be checked
            if (!(rule is IPropertyBoundRule))
                return true;

            // if the rule is property-bound, but no properties are dirty, don't check it
            if (dirtyProperties.Count == 0)
                return false;

            // return true if the rule is bound to any properties that are dirty
            return CollectionUtils.Contains<string>((rule as IPropertyBoundRule).Properties,
                        delegate(string prop) { return dirtyProperties.Contains(prop); });
        }
    }
}
