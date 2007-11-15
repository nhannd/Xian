using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	public class DictionaryEventArgs<TKey, TItem> : EventArgs
	{
		private TKey _key;
		private TItem _item;

		internal protected DictionaryEventArgs()
		{
		}

		public DictionaryEventArgs(TKey key, TItem item)
		{
			Platform.CheckForNullReference(key, "key");
			_key = key;
			_item = item;
		}

		public TKey Key
		{
			get { return _key; }
			internal protected set { _key = value; }
		}

		public TItem Item
		{
			get { return _item; }
			internal protected set { _item = value; }
		}
	}
}
