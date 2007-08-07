using System;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Defines the interface to a table, which provides a presentation model for viewing data in a tabular form.
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// Returns the <see cref="Type"/> of the items in this table.
        /// </summary>
        Type ItemType { get; }

        /// <summary>
        /// Gets the collection of items in the table.  The returned collection is filtered if <see cref="Filter()"/> has been called.  To ensure all items are returned, use <see cref="RemoveFilter()"/> prior to using this property.
        /// </summary>
        /// <remarks>CF: ITable{TItem}.Items which always returns the complete collection</remarks>
        IItemCollection Items { get; }

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

        /// <summary>
        /// Gets a value indicating if the table is filtered or not.
        /// </summary>
        bool IsFiltered { get; }

        /// <summary>
        /// Filters this table according to the cached filter parameters, if any exist.
        /// </summary>
        void Filter();

        /// <summary>
        /// Filters this table accordint ot the specified filter parameters.
        /// </summary>
        /// <param name="filterParams"></param>
        void Filter(TableFilterParams filterParams);

        /// <summary>
        /// Removes the applied filter, if one exists.
        /// </summary>
        void RemoveFilter();

        /// <summary>
        /// Gets the base column width for this table, in units that correspond roughly to the
        /// width of one character.
        /// </summary>
        float BaseColumnWidthChars { get; }
    }

    /// <summary>
    /// Defines an additional interface to a table, which provides generic methods for viewing its data.
    /// Used in conjunction with <see cref="ITable"/>.
    /// </summary>
    public interface ITable<TItem>
    {
        /// <summary>
        /// Gets the collection of items in the table.
        /// </summary>
        /// <remarks>The returned collection is never filtered.  CF: ITable.Items which may return a filtered list</remarks>
        ItemCollection<TItem> Items { get; }

        /// <summary>
        /// Gets the collection of columns.
        /// </summary>
        TableColumnCollection<TItem> Columns { get; }
    }
}
