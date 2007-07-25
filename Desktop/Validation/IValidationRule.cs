using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Defines the interface to a validation rule that is applied to a <see cref="IApplicationComponent"/>.
    /// The <see cref="PropertyName"/> property specifies a property of the application component
    /// that the rule applies to.  Any validation error message will be displayed next to the GUI object
    /// that is bound to this property.
    /// </summary>
    public interface IValidationRule
    {
        /// <summary>
        /// Gets the name of the property on the application component that this rule applies to.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Obtains the current result of evaluating this rule based on the current state of the application component.
        /// </summary>
        ValidationResult GetResult(IApplicationComponent component);
    }
}
