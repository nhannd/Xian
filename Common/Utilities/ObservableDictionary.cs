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
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	public class ObservableDictionary<TKey, TItem> : IDictionary<TKey, TItem>
	{
		private readonly Dictionary<TKey, TItem> _dictionary;
		private event EventHandler<DictionaryEventArgs<TKey, TItem>> _itemAddedEvent;
		private event EventHandler<DictionaryEventArgs<TKey, TItem>> _itemChanging;
		private event EventHandler<DictionaryEventArgs<TKey, TItem>> _itemChanged;
		private event EventHandler<DictionaryEventArgs<TKey, TItem>> _itemRemovedEvent;

		public ObservableDictionary()
		{
			_dictionary = new Dictionary<TKey, TItem>();
		}

		public ObservableDictionary(IDictionary<TKey, TItem> dictionary)
			: this()
		{
			foreach (KeyValuePair<TKey, TItem> pair in dictionary)
				this.Add(pair);
		}

		public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemAdded
		{
			add { _itemAddedEvent += value; }
			remove { _itemAddedEvent -= value; }
		}

		public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemRemoved
		{
			add { _itemRemovedEvent += value; }
			remove { _itemRemovedEvent -= value; }
		}

		public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemChanging
		{
			add { _itemChanging += value; }
			remove { _itemChanging -= value; }
		}

		public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemChanged
		{
			add { _itemChanged += value; }
			remove { _itemChanged -= value; }
		}
		
		#region IDictionary<TKey,TItem> Members

		public void Add(TKey key, TItem value)
		{
			_dictionary.Add(key, value);
			OnItemAdded(new DictionaryEventArgs<TKey, TItem>(key, value));
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
			if (_dictionary.ContainsKey(key))
			{
				DictionaryEventArgs<TKey, TItem> args = new DictionaryEventArgs<TKey, TItem>(key, _dictionary[key]);
				_dictionary.Remove(key);
				OnItemRemoved(args);
				return true;
			}
			
			return false;
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
				if (_dictionary.ContainsKey(key))
				{
					DictionaryEventArgs<TKey, TItem> args = new DictionaryEventArgs<TKey, TItem>(key, _dictionary[key]);
					OnItemChanging(args);

					_dictionary[key] = value;

					args.Item = value;
					OnItemChanged(args);
				}
				else
				{
					Add(key, value);
				}
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TItem>> Members

		public void Add(KeyValuePair<TKey, TItem> item)
		{
			(_dictionary as ICollection<KeyValuePair<TKey, TItem>>).Add(item);
			OnItemAdded(new DictionaryEventArgs<TKey, TItem>(item.Key, item.Value));
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
			bool result = (_dictionary as ICollection<KeyValuePair<TKey, TItem>>).Remove(item);

			// Only raise event if the item was actually removed
			if (result == true)
				OnItemRemoved(new DictionaryEventArgs<TKey, TItem>(item.Key, item.Value));

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

		protected virtual void OnItemAdded(DictionaryEventArgs<TKey, TItem> e)
		{
			EventsHelper.Fire(_itemAddedEvent, this, e);
		}

        protected virtual void OnItemRemoved(DictionaryEventArgs<TKey, TItem> e)
		{
			EventsHelper.Fire(_itemRemovedEvent, this, e);
		}

		protected virtual void OnItemChanging(DictionaryEventArgs<TKey, TItem> e)
		{
			EventsHelper.Fire(_itemChanging, this, e);
		}

		protected virtual void OnItemChanged(DictionaryEventArgs<TKey, TItem> e)
		{
			EventsHelper.Fire(_itemChanged, this, e);
		}
	}
}
