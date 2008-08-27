using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop {
	public delegate void GalleryItemEventHandler(object sender, GalleryItemEventArgs e);

	public class GalleryItemEventArgs : EventArgs {
		public GalleryItemEventArgs(IGalleryItem item)
		{
			_item = item;
		}

		private IGalleryItem _item;
		public IGalleryItem Item
		{
			get { return _item; }
		}
	}
}
