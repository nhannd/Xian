using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Clipboard
{
	public interface IClipboardItem
	{
		object Item { get; }
	}

	internal class ClipboardItem : IClipboardItem, IGalleryItem
	{
		private readonly object _item;
		private readonly Image _image;
		private readonly string _description;
		private readonly Rectangle _displayRectangle;

		public ClipboardItem(object item, Image image, string description, Rectangle displayRectangle)
		{
			_item = item;
			_image = image;
			_description = description;
			_displayRectangle = displayRectangle;
		}

		public object Item
		{
			get { return _item; }
		}

		public Image Image
		{
			get { return _image; }
		}

		public string Description
		{
			get { return _description; }
		}

		public Rectangle DisplayRectangle
		{
			get { return _displayRectangle; }
		}
	}
}
