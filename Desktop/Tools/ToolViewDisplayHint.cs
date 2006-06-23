using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Set of flags that customize the behaviour of tool views.
    /// </summary>
    [FlagsAttribute]
    public enum ToolViewDisplayHint
    {
		None = 0,
		DockLeft = 1,
		DockRight = 2,
		DockTop = 4,
		DockBottom  = 8,
		DockFloat = 16,
		DockAutoHide = 32,
		MaximizeOnDock = 64,
		HideOnWorkspaceOpen = 128
    }
}
