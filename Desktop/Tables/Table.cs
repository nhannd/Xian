#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// A delegate for selecting color based on an object based on
    /// an item in <see cref="ITable.Items"/>.
    /// </summary>
    /// <returns>Name of a predefined color.</returns>
    public delegate string ColorSelector<T>(T item);

    /// <summary>
    /// A useful generic implementation of <see cref="ITable"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of item that this table holds.</typeparam>
    public class Table<TItem> : ITable, ITable<TItem>
    {
        private TableColumnCollection<TItem> _columns;
        private ItemCollection<TItem> _data;
        private float _baseColumnWidth;

        private TableSortParams _sortParams;

        private bool _isFiltered = false;
        private TableFilterParams _filterParams;
        private FilteredItemCollection<TItem> _filteredData;

        private readonly int _cellRowCount;
        private ColorSelector<TItem> _backgroundColorSelector;
        private ColorSelector<TItem> _outlineColorSelector;

    	private EventHandler _beforeSortEvent;
        private EventHandler _sortedEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Table()
            : this(1)
        {
        }

        ///<summary>
        /// Constructs a table with the specified number of cells in each row.
        ///</summary>
        public Table(int cellRowCount)
        {
            Platform.CheckArgumentRange((int)cellRowCount, 1, int.MaxValue, "cellRowCount");
            _cellRowCount = cellRowCount;

            _baseColumnWidth = 10;
            _columns = new TableColumnCollection<TItem>();
            _columns.ItemsChanged += delegate(object sender, ItemChangedEventArgs args)
            {
                switch (args.ChangeType)
                {
                    case ItemChangeType.ItemAdded:
                        _columns[args.ItemIndex].Table = this;
                        break;
                    case ItemChangeType.ItemRemoved:
                        _columns[args.ItemIndex].Table = null;
                        break;
                }
            };

            _data = new ItemCollection<TItem>();
            _filteredData = null;
        }

        /// <summary>
        /// Gets and sets the background color of a cell row.
        /// </summary>
        public ColorSelector<TItem> BackgroundColorSelector
        {
            get { return _backgroundColorSelector; }
            set { _backgroundColorSelector = value; }
        }

        /// <summary>
        /// Gets and sets the outline color of a cell row.
        /// </summary>
        public ColorSelector<TItem> OutlineColorSelector
        {
            get { return _outlineColorSelector; }
            set { _outlineColorSelector = value; }
        }

        #region ITable members

    	/// <summary>
    	/// Returns the <see cref="Type"/> of the items in this table.
    	/// </summary>
    	public Type ItemType
        {
            get { return typeof(TItem); }
        }

    	/// <summary>
    	/// Gets the cached sort parameters, or returns null if this table has not been sorted.
    	/// </summary>
    	public TableSortParams SortParams
        {
            get { return _sortParams; }
        }

    	/// <summary>
    	/// Get the collection of columns.
    	/// </summary>
    	ITableColumnCollection ITable.Columns
        {
            get { return _columns; }
        }

    	/// <summary>
    	/// Sorts this table according to the cached sort parameters, if any exist.
    	/// </summary>
    	public void Sort()
        {
            if (_sortParams != null)
            {
				EventsHelper.Fire(_beforeSortEvent, this, EventArgs.Empty);
				
				if (_isFiltered)
                    _filteredData.Sort(new TypeSafeComparerWrapper<TItem>(_sortParams.Column.GetComparer(_sortParams.Ascending)));
                else 
                    _data.Sort(new TypeSafeComparerWrapper<TItem>(_sortParams.Column.GetComparer(_sortParams.Ascending)));

                EventsHelper.Fire(_sortedEvent, this, EventArgs.Empty);
            }
        }

    	/// <summary>
    	/// Sorts this table according to the specified sort parameters.
    	/// </summary>
    	public void Sort(TableSortParams sortParams)
        {
            _sortParams = sortParams;
            Sort();
        }

		/// <summary>
		/// Raised after the table is sorted.
		/// </summary>
		public event EventHandler BeforeSorted
		{
			add { _beforeSortEvent += value; }
			remove { _beforeSortEvent -= value; }
		}

		/// <summary>
		/// Raised after the table is sorted.
    	/// </summary>
    	public event EventHandler Sorted
        {
            add { _sortedEvent += value; }
            remove { _sortedEvent -= value; }
        }

    	/// <summary>
    	/// Gets the base column width for this table, in units that correspond roughly to the
    	/// width of one character.
    	/// </summary>
    	public float BaseColumnWidthChars
        {
            get { return _baseColumnWidth; }
            set { _baseColumnWidth = value; }
        }

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
    	IItemCollection ITable.Items
        {
            get { return _isFiltered ? _filteredData : _data; }
        }

    	/// <summary>
    	/// Gets a value indicating if the table is filtered or not.
    	/// </summary>
    	public bool IsFiltered
        {
            get { return _isFiltered; }
        }

    	/// <summary>
    	/// Filters this table according to the specified filter parameters.
    	/// </summary>
    	public void Filter(TableFilterParams filterParams)
        {
            _filterParams = filterParams;
            Filter();
        }

    	/// <summary>
    	/// Filters this table according to the cached filter parameters, if any exist.
    	/// </summary>
    	public void Filter()
        {
            if(_filterParams != null && _filterParams.Value != null)
            {
                _isFiltered = true;
                
                _filteredData = new FilteredItemCollection<TItem>(this, _filterParams);
            }
        }

    	/// <summary>
    	/// Removes the applied filter, if one exists.
    	/// </summary>
    	public void RemoveFilter()
        {
            _isFiltered = false;

			// ensure old filtered collections aren't updated when the table is updated.
    		_filteredData.Detach();
    		_filteredData = null;
        }

    	/// <summary>
    	/// Gets the cached filter parameters, or returns null if this table is not filtered.
    	/// </summary>
    	public TableFilterParams FilterParams
		{
			get { return _isFiltered ? _filterParams : null; }
		}

    	/// <summary>
    	/// Gets the number of cell rows in each row.
    	/// </summary>
    	public int CellRowCount
        {
            get { return _cellRowCount; }
        }

        /// <summary>
        /// Gets the color for the background of a cell row.
        /// </summary>
        public string GetItemBackgroundColor(object item)
        {
            return _backgroundColorSelector == null ? null : _backgroundColorSelector((TItem)item);
        }

        /// <summary>
        /// Gets color for the outline of a cell row.
        /// </summary>
        public string GetItemOutlineColor(object item)
        {
            return _outlineColorSelector == null ? null : _outlineColorSelector((TItem)item);
        }


        #endregion

        #region ITable<TItem> members

        /// <summary>
        /// Gets the collection of columns for the table.
        /// </summary>
        /// <remarks>
		/// Use this property to add <see cref="ITableColumn"/> objects.
		/// </remarks>
        public TableColumnCollection<TItem> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Gets the collection of items in the table.  The returned collection is never filtered.
        /// </summary>
        /// <remarks>The returned collection is never filtered.</remarks>
        public ItemCollection<TItem> Items
        {
            get { return _data; }
        }

        #endregion

        #region Internal methods

        internal int GetColumnWidthPercent(TableColumnBase<TItem> column)
        {
            float sum = 0;
            foreach (TableColumnBase<TItem> c in _columns)
            {
                sum += c.WidthFactor;
            }
            return (int)(100 * column.WidthFactor / sum);
        }

        internal void NotifyColumnChanged(TableColumnBase<TItem> column)
        {
            _columns.NotifyItemUpdated(column);
        }

        #endregion

    }
}
