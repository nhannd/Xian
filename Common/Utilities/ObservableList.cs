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

namespace ClearCanvas.Common.Utilities
{
	public class ObservableList<TItem> : IList<TItem>
	{
		private readonly List<TItem> _list;
		
		private event EventHandler<CollectionEventArgs<TItem>> _itemAddedEvent;
		private event EventHandler<CollectionEventArgs<TItem>> _itemRemovedEvent;
		private event EventHandler<CollectionEventArgs<TItem>> _itemChanging; 
		private event EventHandler<CollectionEventArgs<TItem>> _itemChangedEvent;

		public ObservableList()
		{
			_list = new List<TItem>();
		}

		public ObservableList(IEnumerable<TItem> values)
			: this()
		{
			foreach (TItem item in values)
				this.Add(item);
		}

		public void Sort(IComparer<TItem> sortComparer)
		{
			Platform.CheckForNullReference(sortComparer, "sortComparer");

			_list.Sort(sortComparer);
		}

		public virtual event EventHandler<CollectionEventArgs<TItem>> ItemAdded
		{
			add { _itemAddedEvent += value; }
			remove { _itemAddedEvent -= value;	}
		}

		public virtual event EventHandler<CollectionEventArgs<TItem>> ItemRemoved
		{
			add { _itemRemovedEvent += value; }
			remove { _itemRemovedEvent -= value; }
		}

		public virtual event EventHandler<CollectionEventArgs<TItem>> ItemChanged
		{
			add { _itemChangedEvent += value; }
			remove { _itemChangedEvent -= value; }
		}

		public virtual event EventHandler<CollectionEventArgs<TItem>> ItemChanging
		{
			add { _itemChanging += value; }
			remove { _itemChanging -= value; }
		}

		#region IList<T> Members

		public int IndexOf(TItem item)
		{
			Platform.CheckForNullReference(item, "item");

			return _list.IndexOf(item);
		}

		public virtual void Insert(int index, TItem item)
		{
			Platform.CheckArgumentRange(index, 0, this.Count, "index");

			if (_list.Contains(item))
				return;

			_list.Insert(index, item);
			OnItemAdded(new CollectionEventArgs<TItem>(item, index));
		}

		public virtual void RemoveAt(int index)
		{
			Platform.CheckArgumentRange(index, 0, this.Count - 1, "index");

			TItem itemToRemove = this[index];
			_list.RemoveAt(index);

			OnItemRemoved(new CollectionEventArgs<TItem>(itemToRemove, index));
		}

		public TItem this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, this.Count - 1, "index");
				return _list[index];
			}
			set
			{
				Platform.CheckIndexRange(index, 0, this.Count - 1, "index");

				CollectionEventArgs<TItem> args = new CollectionEventArgs<TItem>(_list[index], index);
				OnItemChanging(args);
				
				_list[index] = value;

				args.Item = value;
				OnItemChanged(args);
			}
		}

		#endregion

		#region ICollection<TItem> Members

		public virtual void Add(TItem item)
		{
			if (_list.Contains(item))
				return;

			_list.Add(item);
			OnItemAdded(new CollectionEventArgs<TItem>(item, this.Count - 1));
		}

		public virtual void Clear()
		{
			// If we don't have any subscribers to the ItemRemovedEvent, then
			// make it faster and just call Clear().
			if (_itemRemovedEvent == null)
			{
				_list.Clear();
			}
			// But if we do, then we have to remove items one by one so that
			// subscribers are notified.
			else
			{
				while (this.Count > 0)
				{
					int lastIndex = this.Count - 1;
					RemoveAt(lastIndex);
				}
			}
		}

		public bool Contains(TItem item)
		{
			Platform.CheckForNullReference(item, "item");

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

		public virtual bool Remove(TItem item)
		{
			Platform.CheckForNullReference(item, "item");

			int index = _list.IndexOf(item);

			if (index >= 0)
			{
				_list.RemoveAt(index);
				// Only raise event if the item was actually removed
				OnItemRemoved(new CollectionEventArgs<TItem>(item, index));
				return true;
			}

			return false;
		}

		#endregion

		#region IEnumerable<TItem> Members

		public IEnumerator<TItem> GetEnumerator()
		{
			return (_list as IEnumerable<TItem>).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		#endregion

		protected virtual void OnItemAdded(CollectionEventArgs<TItem> e)
		{
			EventsHelper.Fire(_itemAddedEvent, this, e);
		}

		protected virtual void OnItemRemoved(CollectionEventArgs<TItem> e)
		{
			EventsHelper.Fire(_itemRemovedEvent, this, e);
		}

		private void OnItemChanging(CollectionEventArgs<TItem> e)
		{
			EventsHelper.Fire(_itemChanging, this, e);
		}
		protected virtual void OnItemChanged(CollectionEventArgs<TItem> e)
		{
			EventsHelper.Fire(_itemChangedEvent, this, e);
		}
	}
}
