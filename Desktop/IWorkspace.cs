using System;

using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    public interface IWorkspace
    {
        event EventHandler<ActivationChangedEventArgs> ActivationChangedEvent;
        event EventHandler TitleChanged;

        CommandHistory CommandHistory { get; }
        bool IsActivated { get; set; }
        string Title { get; set; }
        IToolSet ToolSet { get; }
        IWorkspaceView View { get; }

        void Cleanup();
    }
}
