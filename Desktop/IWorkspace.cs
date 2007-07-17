using System;

using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the public interface to a <see cref="Workspace"/>.
    /// </summary>
    public interface IWorkspace : IDesktopObject
    {
        /// <summary>
        /// Gets the desktop window that owns this workspace.
        /// </summary>
        IDesktopWindow DesktopWindow { get; }

        /// <summary>
        /// Gets the hosted component.
        /// </summary>
        object Component { get; }

        /// <summary>
        /// Gets the command history associated with this workspace.
        /// </summary>
        CommandHistory CommandHistory { get; }
 
    }
}
