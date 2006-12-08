using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Defines the interface to a validation rule that is typically applied to a property of a <see cref="IApplicationComponent"/>.
    /// The <see cref="PropertyName"/> property specifies which property of the application component this rule applies to.
    /// </summary>
    public interface IValidationRule
    {
        /// <summary>
        /// The name of the property on the application component that this rule applies to.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Obtains the current result of evaluating this rule based on the current runtime state.
        /// </summary>
        ValidationResult Result { get; }
    }
}
