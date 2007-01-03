using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Implementation of <see cref="ITableColumnCollection"/> for use with the <see cref="Table"/> class.
    /// </summary>
    /// <typeparam name="TItem">The type of item that the table holds</typeparam>
    public class TableColumnCollection<TItem> : ItemCollection<TableColumnBase<TItem>>, ITableColumnCollection
    {
    }
}
