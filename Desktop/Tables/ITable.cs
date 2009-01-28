#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
        /// Gets the collection of items in the table.
        /// </summary>
        /// <remarks>
        /// <para>
		/// The returned collection is filtered if <see cref="Filter()"/> has been called.  To
		/// ensure all items are returned, use <see cref="RemoveFilter()"/> prior to using this property.
		/// </para>
		/// <para>
		/// CF: <see cref="ITable{TItem}.Items"/> which always returns the complete collection.
		/// </para>
		/// </remarks>
        IItemCollection Items { get; }

        /// <summary>
        /// Get the collection of columns.
        /// </summary>
        ITableColumnCollection Columns { get; }

        /// <summary>
        /// Sorts this table according to the cached sort parameters, if any exist.
        /// </summary>
        void Sort();

        /// <summary>
        /// Sorts this table according to the specified sort parameters.
        /// </summary>
        void Sort(TableSortParams sortParams);

		/// <summary>
		/// Raised before the table is sorted.
		/// </summary>
		event EventHandler BeforeSorted;
		
		/// <summary>
		/// Raised after the table is sorted.
        /// </summary>
        event EventHandler Sorted;

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
        void Filter(TableFilterParams filterParams);

        /// <summary>
        /// Removes the applied filter, if one exists.
        /// </summary>
        void RemoveFilter();

		/// <summary>
		/// Gets the cached filter parameters, or returns null if this table has not been filtered.
		/// </summary>
		TableFilterParams FilterParams { get; }

		/// <summary>
        /// Gets the base column width for this table, in units that correspond roughly to the
        /// width of one character.
        /// </summary>
        float BaseColumnWidthChars { get; }

        /// <summary>
        /// Gets the number of cell rows in each row.
        /// </summary>
        int CellRowCount { get; }

        /// <summary>
        /// Gets the color for the background of a cell row.
        /// </summary>
        string GetItemBackgroundColor(object item);

        /// <summary>
		/// Gets color for the outline of a cell row.
        /// </summary>
        string GetItemOutlineColor(object item);
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
		/// <remarks>The returned collection is never filtered.  CF: <see cref="ITable.Items"/> which may return a filtered list.</remarks>
        ItemCollection<TItem> Items { get; }

        /// <summary>
        /// Gets the collection of columns.
        /// </summary>
        TableColumnCollection<TItem> Columns { get; }
    }
}
