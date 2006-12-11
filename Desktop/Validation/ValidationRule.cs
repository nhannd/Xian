using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop.Validation
{

    public delegate object TestValueCallbackDelegate();

    /// <summary>
    /// Implementation of <see cref="IValidationRule"/>.
    /// This class accepts an instance of <see cref="ISpecification"/> which provides the specification which must be satisfied.
    /// The application component instance will be passed to the <see cref="ISpecification.Test"/> method, so the specification
    /// must be written to expect the application component as the root object.
    /// </summary>
    public class ValidationRule : IValidationRule
    {
        private string _propertyName;
        private IApplicationComponent _component;
        private ISpecification _spec;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component">The application component which this rule applies</param>
        /// <param name="propertyName">The property to which this rule applies</param>
        /// <param name="spec">The specification which must be satisfied for this validation rule to succeed</param>
        public ValidationRule(IApplicationComponent component, string propertyName, ISpecification spec)
        {
            _propertyName = propertyName;
            _component = component;
            _spec = spec;
        }

        #region IValidationRule members

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public ValidationResult Result
        {
            get
            {
                TestResult result = _spec.Test(_component);
                return new ValidationResult(result.Success, result.Success ? null : GetTopLevelMessage(result.Reason));
            }
        }

        #endregion

        private string GetTopLevelMessage(TestResultReason reason)
        {
            if (reason == null)
                return null;
            else
                return (reason.Message != null) ? reason.Message : GetTopLevelMessage(reason.Reason);
        }
    }
}
