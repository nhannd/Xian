using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    public interface IDesktopWindow : IDisposable
    {
        IWorkspace ActiveWorkspace { get; }

        ActionModelNode MenuModel { get; }
        ActionModelNode ToolbarModel { get; }

        ShelfManager ShelfManager { get; }
        WorkspaceManager WorkspaceManager { get; }
    }
}
