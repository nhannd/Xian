using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Represents a validation rule that is based on satisfying a specification.
    /// This class accepts an instance of <see cref="ISpecification"/> which provides the specification which must be satisfied.
    /// The application component instance will be passed to the <see cref="ISpecification.Test"/> method, so the specification
    /// must be written to expect the application component as the root object.
    /// </summary>
    public class SpecValidationRule : ValidationRule
    {
        private ISpecification _spec;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component">The application component to test</param>
        /// <param name="propertyName">The property of the component that this rule applies to.  Determines where the error provider will be positioned.</param>
        /// <param name="spec">The specification which must be satisfied for this validation rule to succeed</param>
        public SpecValidationRule(IApplicationComponent component, string propertyName, ISpecification spec)
            : base(component, propertyName)
        {
            _spec = spec;
        }

        protected override ValidationResult Validate(object testValue)
        {
            TestResult result = _spec.Test(testValue);
            return new ValidationResult(result.Success, result.Success ? null : GetTopLevelMessage(result.Reason));
        }

        private string GetTopLevelMessage(TestResultReason reason)
        {
            if (reason == null)
                return null;
            else
                return (reason.Message != null) ? reason.Message : GetTopLevelMessage(reason.Reason);
        }

    }
}
