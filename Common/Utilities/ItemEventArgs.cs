using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	public class ItemEventArgs<TItem> : EventArgs
	{
		private TItem _item;

		public ItemEventArgs(TItem item)
		{
			_item = item;
		}

		protected ItemEventArgs()
		{ 
		}

		public TItem Item
		{
			get { return _item; }
			protected set { _item = value; }
		}
	}
}
