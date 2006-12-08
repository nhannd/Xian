using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{

    public delegate object TestValueCallbackDelegate();

    /// <summary>
    /// Abstract base class for validation rules.  Subclass this class rather than implement <see cref="IValidationRule"/> directly.
    /// </summary>
    public abstract class ValidationRule : IValidationRule
    {
        private string _propertyName;
        private IApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component">The application component which this rule applies</param>
        /// <param name="propertyName">The property to which this rule applies</param>
        public ValidationRule(IApplicationComponent component, string propertyName)
        {
            _propertyName = propertyName;
            _component = component;
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
                return Validate(_component);
            }
        }

        #endregion

        /// <summary>
        /// Called to validate the specified application component.  Subclasses must implement
        /// this method to validate the specified component.
        /// </summary>
        /// <param name="component">The component to validate</param>
        /// <returns>A result indicating whether the component satisfies this validation rule</returns>
        protected abstract ValidationResult Validate(object component);
    }
}
