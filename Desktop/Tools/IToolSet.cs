using System;

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    public interface IToolSet
    {
        ActionModelRoot MenuModel { get; }
        ActionModelRoot ToolbarModel { get; }
        ToolViewProxy[] ToolViews { get; }

        void Activate(bool activate);
    }
}
