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

namespace ClearCanvas.Desktop
{
    ///<summary>
    /// Implements <see cref="IItemCollection{TItem}"/>
    ///</summary>
    ///<typeparam name="TItem">The type of item that the table holds</typeparam>
    public class ItemCollection<TItem> : IItemCollection<TItem>
    {
        private readonly List<TItem> _list;
        private event EventHandler<ItemChangedEventArgs> _itemsChanged;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ItemCollection()
        {
            _list = new List<TItem>();
        }

        protected ItemCollection(ItemCollection<TItem> itemCollection)
        {
            _list = itemCollection.List;
        }

        protected List<TItem> List
        {
            get { return _list; }
        }

        #region IItemCollection Members

        public void NotifyItemUpdated(int index)
        {
            NotifyItemsChanged(ItemChangeType.ItemChanged, index, this[index]);
        }

        public event EventHandler<ItemChangedEventArgs> ItemsChanged
        {
            add { _itemsChanged += value; }
            remove { _itemsChanged -= value; }
        }

        public virtual void AddRange(IEnumerable enumerable)
        {
            if(enumerable is IEnumerable<TItem>)
            {
                AddRange((IEnumerable<TItem>)enumerable);
            }
        }

        #endregion

        #region IItemCollection<TItem> Members

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

        public virtual void AddRange(IEnumerable<TItem> enumerable)
        {
            _list.AddRange(enumerable);
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
        }

        public virtual void Sort(IComparer<TItem> comparer)
        {
            _list.Sort(comparer);

            // notify that the list has been sorted
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
        }

        public virtual void Replace(Predicate<TItem> constraint, TItem newValue)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (constraint(_list[i]))
                {
                    // this assignment will automatically fire a notification event
                    this[i] = newValue;
                }
            }
        }

        public virtual int FindIndex(Predicate<TItem> constraint)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (constraint(_list[i]))
                    return i;
            }
            return -1;
        }

        #endregion

        #region IList Members

        public virtual int Add(object value)
        {
            if (value is TItem)
            {
                Add((TItem)value);
                return IndexOf((TItem) value);
            }
            else return -1;
        }

        public virtual bool Contains(object value)
        {
            if (value is TItem)
            {
                return Contains((TItem)value);
            }
            else return false;
        }

        public virtual void Clear()
        {
            _list.Clear();
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
        }

        public virtual int IndexOf(object value)
        {
            if (value is TItem)
            {
                return IndexOf((TItem)value);
            }
            else return -1;
        }

        public virtual void Insert(int index, object value)
        {
            if(value is TItem)
            {
                Insert(index, (TItem) value);
            }
        }

        public virtual void Remove(object value)
        {
            if(value is TItem)
            {
                Remove((TItem)value);
            }
        }

        public virtual void RemoveAt(int index)
        {
            TItem item = _list[index];
            _list.RemoveAt(index);
            NotifyItemsChanged(ItemChangeType.ItemRemoved, index, item);
        }

        object IList.this[int index]
        {
            get { return ((IList<TItem>)this)[index]; }
            set { ((IList<TItem>)this)[index] = (TItem)value; }
        }

        public virtual bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IList<TItem> Members

        public virtual int IndexOf(TItem item)
        {
            return _list.IndexOf(item);
        }

        public virtual void Insert(int index, TItem item)
        {
            _list.Insert(index, item);
            NotifyItemsChanged(ItemChangeType.ItemAdded, index, item);
        }

        public virtual TItem this[int index]
        {
            get { return _list[index]; }
            set
            {
                _list[index] = value;
                NotifyItemsChanged(ItemChangeType.ItemChanged, index, value);
            }
        }

        #endregion

        #region ICollection Members

        public virtual void CopyTo(Array array, int index)
        {
            if(array is TItem[])
            {
                CopyTo((TItem[])array, index);
            }
        }

        public virtual int Count
        {
            get { return _list.Count; }
        }

        public virtual object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public virtual bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICollection<TItem> Members

        public virtual void Add(TItem item)
        {
            _list.Add(item);
            NotifyItemsChanged(ItemChangeType.ItemAdded, this.Count - 1, item);
        }

        public virtual bool Contains(TItem item)
        {
            return _list.Contains(item);
        }

        public virtual void CopyTo(TItem[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(TItem item)
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

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TItem>) this).GetEnumerator();
        }

        #endregion

        #region IEnumerable<TItem> Members

        public virtual IEnumerator<TItem> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        protected void NotifyItemsChanged(ItemChangeType itemChangeType, int index, TItem item)
        {
            EventsHelper.Fire(_itemsChanged, this, new ItemChangedEventArgs(itemChangeType, index, item));
        }
    }
}
