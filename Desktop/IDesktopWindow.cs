using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to a desktop window, as seen by the <see cref="Application"/>
    /// </summary>
    public interface IDesktopWindow : IDisposable
    {
        /// <summary>
        /// Gets the active workspace, or null if there are no workspaces
        /// </summary>
        IWorkspace ActiveWorkspace { get; }

        /// <summary>
        /// Gets the current menu model
        /// </summary>
        ActionModelNode MenuModel { get; }

        /// <summary>
        /// Gets the current toolbar model
        /// </summary>
        ActionModelNode ToolbarModel { get; }

        /// <summary>
        /// Gets the associated shelf manager
        /// </summary>
        ShelfManager ShelfManager { get; }

        /// <summary>
        /// Gets the associated workspace manager
        /// </summary>
        WorkspaceManager WorkspaceManager { get; }
    }
}
