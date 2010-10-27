#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Defines the interface to provide custom validation strategies for application components.
    /// </summary>
    public interface IApplicationComponentContainerValidationStrategy
    {
        /// <summary>
        /// Determines whether the specified container has validation errors, according to this strategy.
        /// </summary>
        bool HasValidationErrors(IApplicationComponentContainer container);

        /// <summary>
        /// Displays validation errors for the specified container to the user, according to the logic
        /// encapsulated in this strategy.
        /// </summary>
        void ShowValidation(IApplicationComponentContainer container, bool show);
    }
}
