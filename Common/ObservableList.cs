using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Common
{
	public class ObservableList<TItem, TItemEventArgs> 
		: IObservableList<TItem, TItemEventArgs>

		//JC 2006-07-10 Changes for refactoring split
		// mono does not like the class constraint on an interface
		//where TItem : class
		where TItemEventArgs : CollectionEventArgs<TItem>, new()
	{
		private List<TItem> _List = new List<TItem>();
		private event EventHandler<TItemEventArgs> _ItemAddedEvent;
		private event EventHandler<TItemEventArgs> _ItemRemovedEvent;

		public ObservableList()
		{

		}

		public ObservableList(ObservableList<TItem, TItemEventArgs> list)
		{
			foreach (TItem item in list)
				this.Add(item);
		}

		#region IObservableList<TItem, TItemEventArgs> Members

		public event EventHandler<TItemEventArgs> ItemAdded
		{
			add { _ItemAddedEvent += value; }
			remove { _ItemAddedEvent -= value;	}
		}

		public event EventHandler<TItemEventArgs> ItemRemoved
		{
			add { _ItemRemovedEvent += value; }
			remove { _ItemRemovedEvent -= value; }
		}

		#endregion

		#region IList<T> Members

		public int IndexOf(TItem item)
		{
			Platform.CheckForNullReference(item, "item");

			return _List.IndexOf(item);
		}

		public void Insert(int index, TItem item)
		{
			Platform.CheckArgumentRange(index, 0, this.Count - 1, "index");

			if (_List.Contains(item))
				return;

			_List.Insert(index, item);

			TItemEventArgs args = new TItemEventArgs();
			args.Item = item;
			OnItemAdded(args);
		}

		public void RemoveAt(int index)
		{
			Platform.CheckArgumentRange(index, 0, this.Count - 1, "index");

			TItem itemToRemove = this[index];
			_List.RemoveAt(index);

			TItemEventArgs args = new TItemEventArgs();
			args.Item = itemToRemove;
			OnItemRemoved(args);
		}

		public TItem this[int index]
		{
			get
			{
				Platform.CheckIndexRange(index, 0, this.Count - 1, "index");
				return _List[index];
			}
			set
			{
				Platform.CheckIndexRange(index, 0, this.Count - 1, "index");
				_List[index] = value;
			}
		}

		#endregion

		#region ICollection<TItem> Members

		public void Add(TItem item)
		{
			if (_List.Contains(item))
				return;

			_List.Add(item);

			TItemEventArgs args = new TItemEventArgs();
			args.Item = item;
			OnItemAdded(args);
		}

		public void Clear()
		{
			// If we don't have any subscribers to the ItemRemovedEvent, then
			// make it faster and just call Clear().
			if (_ItemRemovedEvent == null)
			{
				_List.Clear();
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

			return _List.Contains(item);
		}

		public void CopyTo(TItem[] array, int arrayIndex)
		{
			_List.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _List.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(TItem item)
		{
			Platform.CheckForNullReference(item, "item");

			bool result = _List.Remove(item);

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
			return (_List as IEnumerable<TItem>).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _List.GetEnumerator();
		}

		#endregion

		protected virtual void OnItemAdded(TItemEventArgs e)
		{
			EventsHelper.Fire(_ItemAddedEvent, this, e);
		}

		protected virtual void OnItemRemoved(TItemEventArgs e)
		{
			EventsHelper.Fire(_ItemRemovedEvent, this, e);
		}
	}
}
