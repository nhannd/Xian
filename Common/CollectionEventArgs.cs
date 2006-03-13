using System;
using System.Text;

namespace ClearCanvas.Common
{
	public class CollectionEventArgs<TItem> : EventArgs
	{
		private TItem m_Item;

		public CollectionEventArgs()
		{
		}

		public TItem Item
		{
			get { return m_Item; }
			set { m_Item = value; }
		}
	}
}
