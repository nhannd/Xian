#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Defines the interface to a set of <see cref="IValidationRule"/> objects.
    /// </summary>
    public interface IValidationRuleSet
    {
        /// <summary>
        /// Adds a rule to the set.
        /// </summary>
        void Add(IValidationRule rule);

        /// <summary>
        /// Removes a rule from the set.
        /// </summary>
        void Remove(IValidationRule rule);

        /// <summary>
        /// Evaluates every rule in the set against the specified component.
        /// </summary>
        /// <param name="component">Component to validate.</param>
        List<ValidationResult> GetResults(IApplicationComponent component);

        /// <summary>
        /// Evaluates all rules in the set that apply to the specified property against the specified component.
        /// </summary>
        /// <param name="component">Component to validate.</param>
        /// <param name="propertyName">Property to validate.</param>
        List<ValidationResult> GetResults(IApplicationComponent component, string propertyName);
    }
}
