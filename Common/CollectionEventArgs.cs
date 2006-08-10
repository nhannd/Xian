using System;
using System.Text;

namespace ClearCanvas.Common
{
	public class CollectionEventArgs<TItem> : EventArgs
	{
		private TItem _Item;

		public CollectionEventArgs()
		{
		}

		public TItem Item
		{
			get { return _Item; }
			set { _Item = value; }
		}
	}
}
