using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
   
    /// <summary>
    /// Implementation of <see cref="IItemCollection"/>.
    /// </summary>
    /// <typeparam name="TItem">The type of item that the table holds</typeparam>
    public class ItemCollection<TItem> : IList<TItem>, IItemCollection
    {

        private List<TItem> _list;
        private event EventHandler<ItemEventArgs> _itemsChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public ItemCollection()
        {
            _list = new List<TItem>();
        }

        /// <summary>
        /// Searches the collection for an item that satisfies the specified constraint and returns
        /// the index of the first such item.
        /// </summary>
        /// <returns>The index of the first matching item, or -1 if no matching items are found</returns>
        public int FindIndex(Predicate<TItem> constraint)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (constraint(_list[i]))
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
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
        }

        /// <summary>
        /// Adds all items in the specified enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        public void AddRange(IEnumerable enumerable)
        {
            _list.AddRange(new TypeSafeEnumerableWrapper<TItem>(enumerable));
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
        }

        /// <summary>
        /// Notifies the table that the item at the specified index has changed in some way.  Use this method
        /// to cause the view to update itself to reflect the changed item.
        /// </summary>
        /// <param name="index"></param>
        public void NotifyItemUpdated(int index)
        {
            NotifyItemsChanged(ItemChangeType.ItemChanged, index, this[index]);
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
                throw new ArgumentException(SR.ExceptionTableItemNotFoundInCollection);
            }
        }

        #region IItemCollection members

        public event EventHandler<ItemEventArgs> ItemsChanged
        {
            add { _itemsChanged += value; }
            remove { _itemsChanged -= value; }
        }

        object IItemCollection.this[int index]
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
            NotifyItemsChanged(ItemChangeType.ItemAdded, index, item);
        }

        public void RemoveAt(int index)
        {
            TItem item = _list[index];
            _list.RemoveAt(index);
            NotifyItemsChanged(ItemChangeType.ItemRemoved, index, item);
        }

        public TItem this[int index]
        {
            get { return _list[index]; }
            set
            {
                _list[index] = value;
                NotifyItemsChanged(ItemChangeType.ItemChanged, index, value);
            }
        }

        #endregion

        #region ICollection<TItem> Members

        public void Add(TItem item)
        {
            _list.Add(item);
            NotifyItemsChanged(ItemChangeType.ItemAdded, this.Count - 1, item);
        }

        public void Clear()
        {
            _list.Clear();
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
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

        bool ICollection<TItem>.IsReadOnly
        {
            get { return (_list as ICollection<TItem>).IsReadOnly; }
        }

        public bool Remove(TItem item)
        {
            int index = IndexOf(item);
            bool removed = _list.Remove(item);
            if (removed)
            {
                NotifyItemsChanged(ItemChangeType.ItemRemoved, index, item);
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

            // notify that the list has been sorted
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
        }

        private void NotifyItemsChanged(ItemChangeType itemChangeType, int index, TItem item)
        {
            EventsHelper.Fire(_itemsChanged, this, new ItemEventArgs(itemChangeType, index, item));
        }
    }
}
