using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Presentation
{
    public interface ITableData
    {
        IList<ITableRow> Rows { get; }
        IList<ITableColumn> Columns { get; }
    }
}
