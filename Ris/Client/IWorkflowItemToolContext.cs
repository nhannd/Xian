using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
    public interface IWorkflowItemToolContext : IToolContext
    {
        bool GetWorkflowOperationEnablement(string operationClass);

        event EventHandler SelectedItemsChanged;
        

        IEnumerable Folders { get; }
        IFolder SelectedFolder { get; }

        IDesktopWindow DesktopWindow { get; }
    }

    public interface IWorkflowItemToolContext<TItem> : IWorkflowItemToolContext
    {
        ICollection<TItem> SelectedItems { get; }
    }
}
