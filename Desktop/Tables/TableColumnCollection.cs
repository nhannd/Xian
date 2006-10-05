using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
    public class TableColumnCollection<TItem> : 
        ObservableList<TableColumnBase<TItem>, CollectionEventArgs<TableColumnBase<TItem>>>,
        ITableColumnCollection
    {
    }
}
