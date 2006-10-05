using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public interface IFolder
    {
        string DisplayName { get; }
        ITable Items { get; }
        event EventHandler ItemsChanged;
    }
}
