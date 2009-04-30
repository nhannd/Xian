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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Adds filtering capablities to <see cref="ItemCollection{TItem}"/> 
    /// for use with <see cref="ITable{TItem}"/>s.
    /// </summary>
    internal class FilteredItemCollection<TItem> : ItemCollection<TItem>, IItemCollection<TItem>
    {
        private readonly TableColumnCollection<TItem> _columns;
        private readonly TableFilterParams _filterParams;
        private readonly List<TItem> _filteredList;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="table">The <see cref="Table{TItem}"/> to filter.</param>
        /// <param name="filterParams">The filter parameters.</param>
        public FilteredItemCollection(ITable<TItem> table, TableFilterParams filterParams)
            : base(table.Items)
        {
            _columns = table.Columns;
            _filterParams = filterParams;

            _filteredList = _filterParams.Column == null
                                ? GetFilteredItemCollection(AnyColumnMatch)
                                : GetFilteredItemCollection(SingleColumnMatch);

            base.ItemsChanged += BaseItemsChanged;
        }

        #region IItemCollection<TItem> Members

		/// <summary>
		/// Adds a range of items to the collection.
		/// </summary>
        public override void AddRange(IEnumerable<TItem> enumerable)
        {
            _filteredList.AddRange(enumerable);
            base.AddRange(enumerable);
        }

		/// <summary>
		/// Sorts the collection using the specified <see cref="IComparer{T}"/>.
		/// </summary>
        public override void Sort(IComparer<TItem> comparer)
        {
            _filteredList.Sort(comparer);
            base.Sort(comparer);
        }

		/// <summary>
		/// Replaces all items in the collection with <paramref name="newValue"/> that
		/// match the input <paramref name="constraint"/>.
		/// </summary>
        public override void Replace(Predicate<TItem> constraint, TItem newValue)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (constraint(_filteredList[i]))
                {
                    // this assignment will automatically fire a notification event
                    this[i] = newValue;
                }
            }

            base.Replace(constraint, newValue);
        }

		/// <summary>
		/// Gets the index of an item in the collection matching the given <param name="constraint"/>.
		/// </summary>
		/// <returns>The index of the item, or -1 if no such item exists.</returns>
		public override int FindIndex(Predicate<TItem> constraint)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (constraint(_filteredList[i]))
                    return i;
            }
            return -1;
        }

        #endregion

        #region IList Members

		/// <summary>
		/// Clears the entire collection.
		/// </summary>
        public override void Clear()
        {
            _filteredList.Clear();
            base.Clear();
        }

		/// <summary>
		/// Removes the item at the specified <paramref name="index"/>.
		/// </summary>
        public override void RemoveAt(int index)
        {
            TItem item = this[index];
            _filteredList.RemoveAt(index);

            base.ItemsChanged -= BaseItemsChanged;
            base.RemoveAt(base.IndexOf(item));
            base.ItemsChanged += BaseItemsChanged;
        }

		/// <summary>
		/// Gets the item at the specified <paramref name="index"/>.
		/// </summary>
        object IList.this[int index]
        {
            get { return ((IList<TItem>) _filteredList)[index]; }
            set { ((IList<TItem>) _filteredList)[index] = (TItem) value; }
        }

        #endregion

        #region IList<TItem> Members

		/// <summary>
		/// Gets the index of an item in the collection.
		/// </summary>
		/// <returns>The index of the item, or -1 if no such item exists.</returns>
		public override int IndexOf(TItem item)
        {
            return _filteredList.IndexOf(item);
        }

		/// <summary>
		/// Inserts <paramref name="index"/> into the collection at the specified <paramref name="index"/>.
		/// </summary>
        public override void Insert(int index, TItem item)
        {
            _filteredList.Insert(index, item);
            // is just adding it to the unfiltered list okay??
            base.ItemsChanged -= BaseItemsChanged;
            base.Add(item);
            base.ItemsChanged += BaseItemsChanged;
        }

		/// <summary>
		/// Gets the item at the specified <paramref name="index"/>.
		/// </summary>
		TItem IList<TItem>.this[int index]
        {
            get { return _filteredList[index]; }
            set 
            {
                TItem item = _filteredList[index];
                _filteredList[index] = value;
                base[base.IndexOf(item)] = value;
            }
        }

        #endregion

        #region ICollection Members

		/// <summary>
		/// Copies the entire contents of the collection to <paramref name="array"/>, 
		/// starting at <paramref name="index"/>.
		/// </summary>
        public override void CopyTo(Array array, int index)
        {
            if (array is TItem[])
            {
                CopyTo((TItem[])array, index);
            }
        }

		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
        public override int Count
        {
            get { return _filteredList.Count; }
        }

        #endregion

        #region ICollection<TItem> Members

		/// <summary>
		/// Adds <paramref name="item"/> to the collection.
		/// </summary>
        public override void Add(TItem item)
        {
            _filteredList.Add(item);

            base.ItemsChanged -= BaseItemsChanged;
            base.Add(item);
            base.ItemsChanged += BaseItemsChanged;
        }

		/// <summary>
		/// Gets whether or not <paramref name="item"/> exists in the collection.
		/// </summary>
        public override bool Contains(TItem item)
        {
            return _filteredList.Contains(item);
        }

		/// <summary>
		/// Copies the entire contents of the collection to <paramref name="array"/>, 
		/// starting at <paramref name="arrayIndex"/>.
		/// </summary>
        public override void CopyTo(TItem[] array, int arrayIndex)
        {
            _filteredList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes <paramref name="item"/> from the collection.
        /// </summary>
        /// <returns>True if the item existed in the collection and was removed, otherwise false.</returns>
		public override bool Remove(TItem item)
        {
            base.ItemsChanged -= BaseItemsChanged;
            base.RemoveAt(base.IndexOf(item));
            base.ItemsChanged += BaseItemsChanged;

            // Bug: item cannot be removed from filtered list until after it is removed from original list
            // Not sure why this is the case, but the code works as written.
            return _filteredList.Remove(item);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets an <see cref="IEnumerator"/> for the collection.
        /// </summary>
		IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TItem>)this).GetEnumerator();
        }

        #endregion

        #region IEnumerable<TItem> Members

		/// <summary>
		/// Gets an <see cref="IEnumerator{TItem}"/> for the collection.
		/// </summary>
		public override IEnumerator<TItem> GetEnumerator()
        {
            return _filteredList.GetEnumerator();
        }

        #endregion

        protected override void NotifyItemsChanged(ItemChangeType itemChangeType, int index, TItem item)
        {
            // This function is called from the base, so the index is the unfiltered item index
            // Need to convert to the filtered item index, or else an exception will be thrown when
            // table adaptor fire a ListChangedEvent
            base.NotifyItemsChanged(itemChangeType, IndexOf(item), item);
        }

        #region Private Methods

		private List<TItem> GetFilteredItemCollection(Predicate<TItem> filter)
        {
            return CollectionUtils.Select<TItem, List<TItem>>(
                this.List,
                delegate(TItem item)
                    {
                        return filter(item);
                    });
        }

        private bool AnyColumnMatch(TItem item)
        {
            string filterValue = _filterParams.Value.ToString().ToLower();

            return CollectionUtils.Contains<TableColumnBase<TItem>>(
                _columns,
                delegate(TableColumnBase<TItem> column)
                    {
                        if (column.GetValue(item) != null)
                        {
                            string columnValue = column.GetValue(item).ToString().ToLower();
                            return columnValue.Contains(filterValue);
                        }
                        else
                        {
                            return false;
                        }
                    });
        }

        private bool SingleColumnMatch(TItem item)
        {
            if (_filterParams.Column.GetValue(item) != null)
            {
                string columnValue = _filterParams.Column.GetValue(item).ToString().ToLower();
                string filterValue = _filterParams.Value.ToString().ToLower();

                return columnValue.Contains(filterValue);
            }
            else
            {
                return false;
            }
        }

        private void BaseItemsChanged(object sender, ItemChangedEventArgs args)
        {
            TItem item = (TItem)args.Item;

            switch (args.ChangeType)
            {
                case ItemChangeType.ItemAdded:
                    bool meetsFilter = _filterParams.Column == null
                                        ? AnyColumnMatch(item)
                                        : SingleColumnMatch(item);

                    if (meetsFilter) _filteredList.Add(item);
                    break;
                case ItemChangeType.ItemRemoved:
                    if (_filteredList.Contains(item)) _filteredList.Remove(item);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}