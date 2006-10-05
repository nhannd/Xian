using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Defines the interface to a table, which provides a presentation model for viewing data in a tabular form.
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// Indicates that one or more items in the <see cref="ITable.Items"/> collection has changed.
        /// </summary>
        event EventHandler<TableItemEventArgs> ItemsChanged;

        /// <summary>
        /// Indicates that one or more columns in the <see cref="ITable.Columns"/> collection has changed.
        /// </summary>
        event EventHandler<TableColumnEventArgs> ColumnsChanged;

        /// <summary>
        /// Returns the <see cref="Type"/> of the items in this table.
        /// </summary>
        Type ItemType { get; }

        /// <summary>
        /// Gets the collection of items
        /// </summary>
        ITableItemCollection Items { get; }

        /// <summary>
        /// Get the collection of columns
        /// </summary>
        ITableColumnCollection Columns { get; }

        /// <summary>
        /// Sorts this table according to the cached sort parameters, if any exist.
        /// </summary>
        void Sort();

        /// <summary>
        /// Sorts this table according to the specified sort parameters.
        /// </summary>
        /// <param name="sortParams"></param>
        void Sort(TableSortParams sortParams);

        /// <summary>
        /// Gets the cached sort parameters, or returns null if this table has not been sorted.
        /// </summary>
        TableSortParams SortParams { get; }

        float BaseColumnWidth { get; }
    }
}
