#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to decorate an action
    /// once it has been declared by an <see cref="ActionInitiatorAttribute"/>.
    /// </summary>
    public abstract class ActionDecoratorAttribute : ActionAttribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The unique identifer of the action.</param>
        protected ActionDecoratorAttribute(string actionID)
            : base(actionID)
        {
        }
    }
}
