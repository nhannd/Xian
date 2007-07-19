using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// A set of flags that indicate how a shelf should be displayed.
    /// </summary>
    [Flags]
    public enum ShelfDisplayHint
    {
        /// <summary>
        /// None
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
		HideOnWorkspaceOpen = 64
    }
}
