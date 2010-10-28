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
    /// Defines the interface for a view onto an action.
    /// </summary>
    public interface IActionView : IView
    {
        /// <summary>
        /// Gets the action view's context; the property is set by the framework.
        /// </summary>
		IActionViewContext Context { get; set; }
    }
}
