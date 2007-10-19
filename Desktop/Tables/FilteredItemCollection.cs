#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class FilteredItemCollection<TItem> : ItemCollection<TItem>, IItemCollection<TItem>
    {
        private readonly TableColumnCollection<TItem> _columns;
        private readonly TableFilterParams _filterParams;
        private readonly List<TItem> _filteredList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table">The <see cref="Table{TItem}"/> to filter</param>
        /// <param name="filterParams">The filter parameters</param>
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

        public override void AddRange(IEnumerable<TItem> enumerable)
        {
            _filteredList.AddRange(enumerable);
            base.AddRange(enumerable);
        }

        public override void Sort(IComparer<TItem> comparer)
        {
            _filteredList.Sort(comparer);
            base.Sort(comparer);
        }

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

        public override void Clear()
        {
            _filteredList.Clear();
            base.Clear();
        }

        public override void RemoveAt(int index)
        {
            TItem item = this[index];
            _filteredList.RemoveAt(index);

            base.ItemsChanged -= BaseItemsChanged;
            base.RemoveAt(base.IndexOf(item));
            base.ItemsChanged += BaseItemsChanged;
        }

        object IList.this[int index]
        {
            get { return ((IList<TItem>) _filteredList)[index]; }
            set { ((IList<TItem>) _filteredList)[index] = (TItem) value; }
        }

        #endregion

        #region IList<TItem> Members

        public override int IndexOf(TItem item)
        {
            return _filteredList.IndexOf(item);
        }

        public override void Insert(int index, TItem item)
        {
            _filteredList.Insert(index, item);
            // is just adding it to the unfiltered list okay??
            base.ItemsChanged -= BaseItemsChanged;
            base.Add(item);
            base.ItemsChanged += BaseItemsChanged;
        }

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

        public override void CopyTo(Array array, int index)
        {
            if (array is TItem[])
            {
                CopyTo((TItem[])array, index);
            }
        }

        public override int Count
        {
            get { return _filteredList.Count; }
        }

        #endregion

        #region ICollection<TItem> Members

        public override void Add(TItem item)
        {
            _filteredList.Add(item);

            base.ItemsChanged -= BaseItemsChanged;
            base.Add(item);
            base.ItemsChanged += BaseItemsChanged;
        }

        public override bool Contains(TItem item)
        {
            return _filteredList.Contains(item);
        }

        public override void CopyTo(TItem[] array, int arrayIndex)
        {
            _filteredList.CopyTo(array, arrayIndex);
        }

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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TItem>)this).GetEnumerator();
        }

        #endregion

        #region IEnumerable<TItem> Members

        public override IEnumerator<TItem> GetEnumerator()
        {
            return _filteredList.GetEnumerator();
        }

        #endregion


        #region Private Methods

        internal List<TItem> GetFilteredItemCollection(Predicate<TItem> filter)
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