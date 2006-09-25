using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Common.Utilities
{
	public class ObservableList<TItem, TItemEventArgs> 
		: IObservableList<TItem, TItemEventArgs>

		//JC 2006-07-10 Changes for refactoring split
		// mono does not like the class constraint on an interface
		//where TItem : class
		where TItemEventArgs : CollectionEventArgs<TItem>, new()
	{
		private List<TItem> _list = new List<TItem>();
		
		private event EventHandler<TItemEventArgs> _itemAddedEvent;
		private event EventHandler<TItemEventArgs> _itemRemovedEvent;

		public ObservableList()
		{

		}

		public ObservableList(ObservableList<TItem, TItemEventArgs> list)
		{
			foreach (TItem item in list)
				this.Add(item);
		}

		public void Sort(IComparer<TItem> sortComparer)
		{
			Platform.CheckForNullReference(sortComparer, "sortComparer");

			_list.Sort(sortComparer);
		}

		#region IObservableList<TItem, TItemEventArgs> Members

		public virtual event EventHandler<TItemEventArgs> ItemAdded
		{
			add { _itemAddedEvent += value; }
			remove { _itemAddedEvent -= value;	}
		}

		public virtual event EventHandler<TItemEventArgs> ItemRemoved
		{
			add { _itemRemovedEvent += value; }
			remove { _itemRemovedEvent -= value; }
		}

		#endregion

		#region IList<T> Members

		public int IndexOf(TItem item)
		{
			Platform.CheckForNullReference(item, "item");

			return _list.IndexOf(item);
		}

		public virtual void Insert(int index, TItem item)
		{
			Platform.CheckArgumentRange(index, 0, this.Count - 1, "index");

			if (_list.Contains(item))
				return;

			_list.Insert(index, item);

			TItemEventArgs args = new TItemEventArgs();
			args.Item = item;
			OnItemAdded(args);
		}

		public virtual void RemoveAt(int index)
		{
			Platform.CheckArgumentRange(index, 0, this.Count - 1, "index");

			TItem itemToRemove = this[index];
			_list.RemoveAt(index);

			TItemEventArgs args = new TItemEventArgs();
			args.Item = itemToRemove;
			OnItemRemoved(args);
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
				_list[index] = value;
			}
		}

		#endregion

		#region ICollection<TItem> Members

		public virtual void Add(TItem item)
		{
			if (_list.Contains(item))
				return;

			_list.Add(item);

			TItemEventArgs args = new TItemEventArgs();
			args.Item = item;
			OnItemAdded(args);
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

			bool result = _list.Remove(item);

			// Only raise event if the item was actually removed
			if (result == true)
			{
				TItemEventArgs args = new TItemEventArgs();
				args.Item = item;
				OnItemRemoved(args);
			}

			return result;
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

		protected virtual void OnItemAdded(TItemEventArgs e)
		{
			EventsHelper.Fire(_itemAddedEvent, this, e);
		}

		protected virtual void OnItemRemoved(TItemEventArgs e)
		{
			EventsHelper.Fire(_itemRemovedEvent, this, e);
		}
	}
}
