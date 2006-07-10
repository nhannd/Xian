using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Presentation
{
    public interface ITableColumn
    {
        string Header { get; }
        float Width { get; }
    }
}
