using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Implementation of <see cref="ITableItemCollection"/> for use with the <see cref="Table"/> class.
    /// </summary>
    /// <typeparam name="TItem">The type of item that the table holds</typeparam>
    public class TableItemCollection<TItem> : IList<TItem>, ITableItemCollection
    {

        private List<TItem> _list;
        private Table<TItem> _owner;

        /// <summary>
        /// Constructor
        /// </summary>
        public TableItemCollection(Table<TItem> owner)
        {
            _owner = owner;
            _list = new List<TItem>();
        }

        /// <summary>
        /// Searches the collection for an item that satisfies the specified predicate and returns
        /// the index of the first such item.
        /// </summary>
        /// <returns>The index of the first matching item, or -1 if no matching items are found</returns>
        public int FindIndex(Predicate<TItem> findDelegate)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (findDelegate(_list[i]))
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// Adds all items in the specified enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        public void AddRange(IEnumerable<TItem> enumerable)
        {
            _list.AddRange(enumerable);
            _owner.NotifyDataChanged(TableItemChangeType.Reset, -1);
        }

        /// <summary>
        /// Adds all items in the specified enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        public void AddRange(IEnumerable enumerable)
        {
            _list.AddRange(new TypeSafeEnumerableWrapper<TItem>(enumerable));
            _owner.NotifyDataChanged(TableItemChangeType.Reset, -1);
        }

        /// <summary>
        /// Notifies the table that the item at the specified index has changed in some way.  Use this method
        /// to cause the view to update itself to reflect the changed item.
        /// </summary>
        /// <param name="index"></param>
        public void NotifyItemUpdated(int index)
        {
            _owner.NotifyDataChanged(TableItemChangeType.ItemChanged, index);
        }

        /// <summary>
        /// Notifies the table that the specified item has changed in some way.  Use this method
        /// to cause the view to update itself to reflect the changed item.
        /// </summary>
        /// <param name="item"></param>
        public void NotifyItemUpdated(TItem item)
        {
            int index = this.IndexOf(item);
            if (index > -1)
            {
                NotifyItemUpdated(index);
            }
            else
            {
                throw new ArgumentException("Item not found in collection");
            }
        }

        #region ITableData members


        object ITableItemCollection.this[int index]
        {
            get { return _list[index]; }
        }

	
        #endregion

        #region IList<TItem> Members

        public int IndexOf(TItem item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TItem item)
        {
            _list.Insert(index, item);
            _owner.NotifyDataChanged(TableItemChangeType.ItemAdded, index);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            _owner.NotifyDataChanged(TableItemChangeType.ItemRemoved, index);
        }

        public TItem this[int index]
        {
            get { return _list[index]; }
            set
            {
               _list[index] = value;
               _owner.NotifyDataChanged(TableItemChangeType.ItemChanged, index);
            }
        }

        #endregion

        #region ICollection<TItem> Members

        public void Add(TItem item)
        {
            _list.Add(item);
            _owner.NotifyDataChanged(TableItemChangeType.ItemAdded, this.Count - 1);
        }

        public void Clear()
        {
            _list.Clear();
            _owner.NotifyDataChanged(TableItemChangeType.Reset, -1);
        }

        public bool Contains(TItem item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TItem item)
        {
            int index = IndexOf(item);
            bool removed = _list.Remove(item);
            if (removed)
            {
                _owner.NotifyDataChanged(TableItemChangeType.ItemRemoved, index);
            }
            return removed;
        }

        #endregion

        #region IEnumerable<TItem> Members

        public IEnumerator<TItem> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        internal void Sort(IComparer<TItem> comparer)
        {
            _list.Sort(comparer);
        }
    }
}
