#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Actions
{
    /// <summary>
    /// Performs an action using an implementation specific context.
    /// </summary>
    public interface IActionItem<T>
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">An implementation specific context for the action.</param>
        /// <returns>true on success, false on failure.</returns>
        bool Execute(T context);

        /// <summary>
        /// A descriptive reason for a failure of the action.  This property is populated when <see cref="IActionItem{T}.Execute"/> returns false.
        /// </summary>
        string FailureReason { get; }
    }
}