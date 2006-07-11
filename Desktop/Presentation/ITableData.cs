using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Presentation
{
    public interface ITableData
    {
        ITableRow[] Rows { get; }
        ITableColumn[] Columns { get; }
    }
}
