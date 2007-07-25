using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Default implementation of <see cref="IValidationRule"/>.
    /// </summary>
    public class ValidationRule : IValidationRule
    {
        public delegate ValidationResult ValidationDelegate(IApplicationComponent component);

        private string _propertyName;
        private ValidationDelegate _callback;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertyName">The property to which this rule applies.</param>
        /// <param name="callback">A method that performs the validation.</param>
        public ValidationRule(string propertyName, ValidationDelegate callback)
        {
            Platform.CheckForNullReference(propertyName, "propertyName");
            Platform.CheckForNullReference(callback, "callback");

            _propertyName = propertyName;
            _callback = callback;
        }

        /// <summary>
        /// Constructor that accepts an instance of a <see cref="ISpecification"/>.  The specification
        /// defines the validation rule.
        /// </summary>
        /// <param name="propertyName">The property to which the rule applies.</param>
        /// <param name="spec">The specification to use to evaluate the rule.</param>
        public ValidationRule(string propertyName, ISpecification spec)
            : this(propertyName, (new SpecificationEvaluator(spec)).Evaluate)
        {
        }

        #region IValidationRule members

        /// <summary>
        /// Gets the name of the property on the application component that this rule applies to.
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /// <summary>
        /// Obtains the current result of evaluating this rule based on the current state of the application component.
        /// </summary>
        public ValidationResult GetResult(IApplicationComponent component)
        {
            return _callback(component);
        }

        #endregion

        #region SpecificationEvaluator

        /// <summary>
        /// Helper class for evaluating a specification as a validation rule.
        /// </summary>
        class SpecificationEvaluator
        {
            private ISpecification _specification;

            public SpecificationEvaluator(ISpecification specification)
            {
                _specification = specification;
            }

            public ValidationResult Evaluate(IApplicationComponent component)
            {
                TestResult result = _specification.Test(component);
                return new ValidationResult(result.Success, result.Success ? null : GetTopLevelMessage(result.Reason));
            }

            private static string GetTopLevelMessage(TestResultReason reason)
            {
                if (reason == null)
                    return null;
                else
                    return (reason.Message != null) ? reason.Message : GetTopLevelMessage(reason.Reason);
            }
        }

        #endregion
    }
}
