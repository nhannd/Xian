#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// A set of flags that indicate how a shelf should be displayed.
    /// </summary>
    [Flags]
    public enum ShelfDisplayHint
    {
        /// <summary>
        /// None.
        /// </summary>
		None = 0,

        /// <summary>
        /// Dock the shelf on the left.
        /// </summary>
		DockLeft = 1,

        /// <summary>
        /// Dock the shelf on the right.
        /// </summary>
		DockRight = 2,

        /// <summary>
        /// Dock the shelf at the top.
        /// </summary>
		DockTop = 4,

        /// <summary>
        /// Dock the shelf at the bottom.
        /// </summary>
		DockBottom  = 8,

        /// <summary>
        /// Float the shelf.
        /// </summary>
		DockFloat = 16,

        /// <summary>
        /// Dock the shelf in auto-hide mode.
        /// </summary>
		DockAutoHide = 32,

        /// <summary>
        /// Hide the shelf whenever a new workspace opens.
        /// </summary>
		HideOnWorkspaceOpen = 64,

		/// <summary>
		/// Show the shelf floating (<see cref="DockFloat"/>) near the mouse.
		/// </summary>
		ShowNearMouse = 144 //128 + 16 (DockFloat)
    }
}
