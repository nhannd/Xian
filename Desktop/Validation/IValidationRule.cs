#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Defines the interface to a validation rule that is applied to a <see cref="IApplicationComponent"/>.
    /// </summary>
    /// <remarks>
	/// The <see cref="PropertyName"/> property specifies a property of the application component
	/// that the rule applies to.  Any validation error message will be displayed next to the GUI object
	/// that is bound to this property.
    /// </remarks>
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
