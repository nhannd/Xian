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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Default implementation of <see cref="IItemCollection{TItem}"/>.
    /// </summary>
	/// <remarks>
	/// Do not subclass this class.  It is likely to be removed
	/// in subsequent versions of the framework and is 
	/// not considered part of the public API.
	/// </remarks>
    ///<typeparam name="TItem">The type of item that the table holds.</typeparam>
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

		/// <summary>
		/// Copy constructor.
		/// </summary>
        protected ItemCollection(ItemCollection<TItem> itemCollection)
        {
            _list = itemCollection.List;
        }

		/// <summary>
		/// Gets the internal list of items.
		/// </summary>
        protected List<TItem> List
        {
            get { return _list; }
        }

        #region IItemCollection Members

    	/// <summary>
    	/// Notifies the table that the item at the specified index has changed in some way.
    	/// </summary>
    	/// <remarks>
    	/// Use this method to cause the view to update itself to reflect the changed item.
    	/// </remarks>
    	public void NotifyItemUpdated(int index)
        {
            NotifyItemsChanged(ItemChangeType.ItemChanged, index, this[index]);
        }

    	/// <summary>
    	/// Occurs when an item in the collection has changed.
    	/// </summary>
    	public event EventHandler<ItemChangedEventArgs> ItemsChanged
        {
            add { _itemsChanged += value; }
            remove { _itemsChanged -= value; }
        }

        #endregion

        #region IItemCollection<TItem> Members

    	/// <summary>
    	/// Notifies the table that the specified item has changed in some way.
    	/// </summary>
    	/// <remarks>
    	/// Use this method to cause the view to update itself to reflect the changed item.
    	/// </remarks>
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

    	/// <summary>
    	/// Adds all items in the specified enumeration.
    	/// </summary>
    	public virtual void AddRange(IEnumerable<TItem> enumerable)
        {
            _list.AddRange(enumerable);
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
        }

    	/// <summary>
    	/// Sorts items in the collection using the specified <see cref="IComparer{TItem}"/>.
    	/// </summary>
    	public virtual void Sort(IComparer<TItem> comparer)
        {
            // We don't call _list.Sort(...) because .NET internally uses an unstable sort
			MergeSort(comparer, _list, 0, _list.Count);

            // notify that the list has been sorted
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
        }

    	/// <summary>
    	/// Sets any items in the collection matching the specified constraint to the specified new value. 
    	/// </summary>
    	/// <param name="constraint">A predicate against which all items in the collection will be compared, and replaced with the new value.</param>
    	/// <param name="newValue">The new value with which to replace all matching items in the collection.</param>
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

    	/// <summary>
    	/// Searches the collection for an item that satisfies the specified constraint and returns
    	/// the index of the first such item.
    	/// </summary>
    	/// <returns>The index of the first matching item, or -1 if no matching items are found.</returns>
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

		/// <summary>
		/// Adds an item to the collection, returning the index of the item's position in the collection.
		/// </summary>
		/// <remarks>
		/// The method returns -1 if the item is not of the correct type.
		/// </remarks>
        public virtual int Add(object value)
        {
            if (value is TItem)
            {
                Add((TItem)value);
                return IndexOf((TItem) value);
            }
            else return -1;
        }

		/// <summary>
		/// Gets whether or not the item is in the collection.
		/// </summary>
        public virtual bool Contains(object value)
        {
            if (value is TItem)
            {
                return Contains((TItem)value);
            }
            else return false;
        }

		/// <summary>
		/// Clears the collection.
		/// </summary>
        public virtual void Clear()
        {
            _list.Clear();
            NotifyItemsChanged(ItemChangeType.Reset, -1, default(TItem));
        }

		/// <summary>
		/// Gets the index of the item in the collection, or -1 if it doesn't exist.
		/// </summary>
        public virtual int IndexOf(object value)
        {
            if (value is TItem)
            {
                return IndexOf((TItem)value);
            }
            else return -1;
        }

		/// <summary>
		/// Inserts the specified item at the given index.
		/// </summary>
        public virtual void Insert(int index, object value)
        {
            if(value is TItem)
            {
                Insert(index, (TItem) value);
            }
        }

		/// <summary>
		/// Removes the specified item from the collection.
		/// </summary>
        public virtual void Remove(object value)
        {
            if(value is TItem)
            {
                Remove((TItem)value);
            }
        }

		/// <summary>
		/// Removes the item at the specified index.
		/// </summary>
        public virtual void RemoveAt(int index)
        {
            TItem item = _list[index];
            _list.RemoveAt(index);
            NotifyItemsChanged(ItemChangeType.ItemRemoved, index, item);
        }

		/// <summary>
		/// gets the item at the specified index.
		/// </summary>
        object IList.this[int index]
        {
            get { return ((IList<TItem>)this)[index]; }
            set { ((IList<TItem>)this)[index] = (TItem)value; }
        }

		/// <summary>
		/// Not implemented.
		/// </summary>
        public virtual bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

		/// <summary>
		/// Not implemented.
		/// </summary>
		public virtual bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IList<TItem> Members

		/// <summary>
		/// Gets the index of the specified item, or -1 if it doesn't exist.
		/// </summary>
        public virtual int IndexOf(TItem item)
        {
            return _list.IndexOf(item);
        }

		/// <summary>
		/// Inserts the specified item at the given index.
		/// </summary>
        public virtual void Insert(int index, TItem item)
        {
            _list.Insert(index, item);
            NotifyItemsChanged(ItemChangeType.ItemInserted, index, default(TItem));
        }

		/// <summary>
		/// Gets the item at the given index.
		/// </summary>
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

		/// <summary>
		/// Copies the entire contents of the collection to <paramref name="array"/>, starting at <paramref name="index"/>.
		/// </summary>
        public virtual void CopyTo(Array array, int index)
        {
            if(array is TItem[])
            {
                CopyTo((TItem[])array, index);
            }
        }

		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		public virtual int Count
        {
            get { return _list.Count; }
        }

		/// <summary>
		/// Not implemented.
		/// </summary>
        public virtual object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

		/// <summary>
		/// Not implemented.
		/// </summary>
		public virtual bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICollection<TItem> Members

        /// <summary>
        /// Adds the specified item to the collection.
        /// </summary>
		public virtual void Add(TItem item)
        {
            _list.Add(item);
            NotifyItemsChanged(ItemChangeType.ItemAdded, this.Count - 1, item);
        }

        /// <summary>
        /// Gets whether or not the collection contains the specified item.
        /// </summary>
		public virtual bool Contains(TItem item)
        {
            return _list.Contains(item);
        }

		/// <summary>
		/// Copies the entire contents of the collection to the specified <paramref name="array"/>, starting
		/// at the given <paramref name="arrayIndex"/>.
		/// </summary>
        public virtual void CopyTo(TItem[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified item from the collection.
        /// </summary>
        /// <returns>True if the item existed in the collection and was removed, otherwise false.</returns>
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

		/// <summary>
		/// Gets an <see cref="IEnumerator"/> for the collection.
		/// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TItem>) this).GetEnumerator();
        }

        #endregion

        #region IEnumerable<TItem> Members

		/// <summary>
		/// Gets an <see cref="IEnumerator{TItem}"/> for the collection.
		/// </summary>
		public virtual IEnumerator<TItem> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

		/// <summary>
		/// Raises the <see cref="ItemsChanged"/> event.
		/// </summary>
        protected virtual void NotifyItemsChanged(ItemChangeType itemChangeType, int index, TItem item)
        {
            EventsHelper.Fire(_itemsChanged, this, new ItemChangedEventArgs(itemChangeType, index, item));
		}

		#region Stable Sort Implementation

		/// <summary>
		/// Performs a stable merge sort on the given <paramref name="list"/> using the given <paramref name="comparer"/>.
		/// The range of items sorted is [<paramref name="rangeStart"/>, <paramref name="rangeStop"/>).
		/// </summary>
    	private static void MergeSort(IComparer<TItem> comparer, IList<TItem> list, int rangeStart, int rangeStop)
    	{
    		int rangeLength = rangeStop - rangeStart;
    		if (rangeLength > 1)
    		{
    			// sort halves
    			int rangeMid = rangeStart + rangeLength/2;
    			MergeSort(comparer, list, rangeStart, rangeMid);
    			MergeSort(comparer, list, rangeMid, rangeStop);

    			// merge halves
    			List<TItem> merged = new List<TItem>(rangeLength);
    			int j = rangeStart;
    			int k = rangeMid;

    			for (int n = 0; n < rangeLength; n++)
    			{
					// if left half has run out of items, add item from right half
					// else if right half has run out of items, add item from left half
					// else if the left item is before or equal to the right item, add left half item
					// else add right half item
    				if (k >= rangeStop || (j < rangeMid && comparer.Compare(list[j], list[k]) <= 0))
    					merged.Add(list[j++]);
    				else
    					merged.Add(list[k++]);
    			}

				// copy merged to list
    			k = rangeStart;
    			foreach (TItem item in merged)
    				list[k++] = item;
    		}
    	}

		#endregion
	}
}
