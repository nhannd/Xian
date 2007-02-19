using System;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	public class CollectionEventArgs<TItem> : EventArgs
	{
		private TItem _item;
		private int _index;

		public CollectionEventArgs()
		{
		}

		public TItem Item
		{
			get { return _item; }
			set { _item = value; }
		}

		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}
	}
}
