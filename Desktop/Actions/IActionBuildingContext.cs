#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Used by the action attribute mechanism to maintain context 
    /// between attributes that co-operate to build the same action.
    /// </summary>
    public interface IActionBuildingContext
    {
        /// <summary>
        /// Gets or sets the action that is being built.
        /// </summary>
        Action Action { get; set; }

        /// <summary>
        /// Gets the resource resolver that is supplied to the action.
        /// </summary>
        IResourceResolver ResourceResolver { get; }

        /// <summary>
        /// Gets the target object on which the action operates.
        /// </summary>
        object ActionTarget { get; }

        /// <summary>
        /// Gets the logical action ID for the action.
        /// </summary>
        string ActionID { get; }
	}
}
