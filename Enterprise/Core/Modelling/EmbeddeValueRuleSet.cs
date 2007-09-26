using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common.Specifications;
using System.Collections;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    internal class EmbeddedValueRuleSet : IValidationRuleSet, IPropertyBoundRule
    {
        private ValidationRuleSet _innerRules;
        private PropertyInfo _property;
        private bool _collection;

        public EmbeddedValueRuleSet(PropertyInfo property, ValidationRuleSet innerRules, bool collection)
        {
            _property = property;
            _innerRules = innerRules;
            _collection = collection;
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

        #region IPropertyBoundRule Members

        public PropertyInfo[] Properties
        {
            get { return new PropertyInfo[] { _property }; }
        }

        #endregion

        protected TestResult TestCore(object obj, Predicate<ISpecification> filter)
        {
            object propertyValue = _property.GetGetMethod().Invoke(obj, null);

            // if the propertyValue is null, return true
            // this seems counter-intuitive, but what we are effectively saying is that the rules
            // are bound to the propertyValue being tested - if there is no propertyValue, there are no rules to test
            if (propertyValue == null)
                return new TestResult(true);

            if (_collection)
            {
                // apply to items rather than to the collection
                foreach (object item in (propertyValue as IEnumerable))
                {
                    TestResult result = _innerRules.Test(item, filter);
                    // if any item fails, don't bother testing the rest of the items
                    if (result.Fail)
                    {
                        string message = string.Format(SR.RuleEmbeddeValueCollection, TerminologyTranslator.Translate(_property));
                        return new TestResult(false, new TestResultReason(message, result.Reasons));
                    }
                }
                return new TestResult(true);
            }
            else
            {
                TestResult result = _innerRules.Test(propertyValue, filter);
                if (result.Fail)
                {
                    string message = string.Format(SR.RuleEmbeddeValue, TerminologyTranslator.Translate(_property));
                    return new TestResult(false, new TestResultReason(message, result.Reasons));
                }
                return new TestResult(true);
            }
        }
    }
}
