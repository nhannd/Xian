#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Implementation of <see cref="ITableColumnCollection"/> for use with the <see cref="Table{TItem}"/> class.
    /// </summary>
    /// <typeparam name="TItem">The type of item that the table holds.</typeparam>
    public class TableColumnCollection<TItem> : ItemCollection<TableColumnBase<TItem>>, ITableColumnCollection
    {
    }
}
