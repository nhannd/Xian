using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	public class DisplaySetItem : IGalleryItem
	{
		private readonly IDisplaySet _displaySet;
		private readonly Image _image;

		public DisplaySetItem(IDisplaySet displaySet, Image image)
		{
			_displaySet = displaySet;
			_image = image;
		}

		public Image Image
		{
			get { return _image; }
		}

		public string Description
		{
			get { return _displaySet.Name; }
		}

		public object Item
		{
			get { return _displaySet; }
		}
	}
}
