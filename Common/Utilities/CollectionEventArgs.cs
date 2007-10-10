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
			//TODO: make this protected internal so that only inheritors or classes internal to this project can set the property.
			set { _item = value; }
		}

		public int Index
		{
			get { return _index; }
			//TODO: make this protected internal so that only inheritors or classes internal to this project can set the property.
			set { _index = value; }
		}
	}
}
