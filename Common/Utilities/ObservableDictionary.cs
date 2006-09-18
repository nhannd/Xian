using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	public class ObservableDictionary<TKey, TItem, TItemEventArgs> : IDictionary<TKey, TItem>
		where TItem : class
		where TItemEventArgs : CollectionEventArgs<TItem>, new()
	{
		private Dictionary<TKey,TItem> _dictionary = new Dictionary<TKey,TItem>();
		private event EventHandler<TItemEventArgs> _itemAddedEvent;
		private event EventHandler<TItemEventArgs> _itemRemovedEvent;

		public event EventHandler<TItemEventArgs> ItemAdded
		{
			add { _itemAddedEvent += value; }
			remove { _itemAddedEvent -= value; }
		}

		public event EventHandler<TItemEventArgs> ItemRemoved
		{
			add { _itemRemovedEvent += value; }
			remove { _itemRemovedEvent -= value; }
		}

		#region IDictionary<TKey,TItem> Members

		public void Add(TKey key, TItem value)
		{
			_dictionary.Add(key, value);

			TItemEventArgs args = new TItemEventArgs();
			args.Item = value;
			OnItemAddedEventHandler(args);
		}

		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get { return _dictionary.Keys; }
		}

		public bool Remove(TKey key)
		{
			TItemEventArgs args = new TItemEventArgs();
			args.Item = _dictionary[key];

			bool result = _dictionary.Remove(key);

			// Only raise event if the item was actually removed
			if (result == true)
				OnItemRemovedEventHandler(args);

			return result;
		}

		public bool TryGetValue(TKey key, out TItem value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public ICollection<TItem> Values
		{
			get { return _dictionary.Values; }
		}

		public TItem this[TKey key]
		{
			get
			{
				return _dictionary[key];
			}
			set
			{
				_dictionary[key] = value;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TItem>> Members

		public void Add(KeyValuePair<TKey, TItem> item)
		{
			(_dictionary as ICollection<KeyValuePair<TKey, TItem>>).Add(item);

			TItemEventArgs args = new TItemEventArgs();
			args.Item = item.Value;
			OnItemAddedEventHandler(args);
		}

		public void Clear()
		{
			// If we don't have any subscribers to the ItemRemovedEvent, then
			// make it faster and just call Clear().
			if (_itemRemovedEvent == null)
			{
				_dictionary.Clear();
			}
			// But if we do, then remove items one by one...a little tricky with a dictionary.
			else
			{
				// Normally, if we want to remove all the elements in an indexed collection 
				// (like an array or list) one by one, we iterate backwards through it, removing
				// the last item during each iteration.  But, since there's no concept of
				// an index in a dictionary, we can't do that.  And we can't "foreach" through
				// the dictionary either, since foreach requires that we not be changing
				// the collection while we're iterating (which we'd be doing, since we'd
				// be removing an item each itereation).  So, instead, we copy the dictionary
				// (expensive, I know) into a collection, then iterate through that using
				// the keys as indices into the dictionary, removing items one at a time.

				KeyValuePair<TKey, TItem>[] pairs = new KeyValuePair<TKey, TItem>[_dictionary.Count];
				(_dictionary as ICollection<KeyValuePair<TKey, TItem>>).CopyTo(pairs, 0);

				foreach (KeyValuePair<TKey, TItem> pair in pairs)
					Remove(pair.Key);
			}
		}

		public bool Contains(KeyValuePair<TKey, TItem> item)
		{
			return (_dictionary as ICollection<KeyValuePair<TKey, TItem>>).Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TItem>[] array, int arrayIndex)
		{
			(_dictionary as ICollection<KeyValuePair<TKey, TItem>>).CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return (_dictionary as ICollection<KeyValuePair<TKey, TItem>>).IsReadOnly; }
		}

		public bool Remove(KeyValuePair<TKey, TItem> item)
		{
			TItemEventArgs args = new TItemEventArgs();
			args.Item = item.Value;

			bool result = (_dictionary as ICollection<KeyValuePair<TKey, TItem>>).Remove(item);

			// Only raise event if the item was actually removed
			if (result == true)
				OnItemRemovedEventHandler(args);

			return result;
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TItem>> Members

		public IEnumerator<KeyValuePair<TKey, TItem>> GetEnumerator()
		{
			return (_dictionary as IEnumerable<KeyValuePair<TKey, TItem>>).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		#endregion

		protected virtual void OnItemAddedEventHandler(TItemEventArgs e)
		{
			EventsHelper.Fire(_itemAddedEvent, this, e);
		}

        protected virtual void OnItemRemovedEventHandler(TItemEventArgs e)
		{
			EventsHelper.Fire(_itemRemovedEvent, this, e);
		}
	}
}
