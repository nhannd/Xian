using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    public interface IFolder
    {
        string DisplayName { get; }
        ITableData Items { get; }
        event EventHandler ItemsChanged;
    }
}
