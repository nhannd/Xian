using System.Drawing;
using System.Threading;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Clipboard
{
	public interface IClipboardItem
	{
		object Item { get; }

		// stops the user from clearing the item while it's in use,
		// for example by another thread.
		bool Locked { get; }

		void Lock();
		
		void Unlock();
	}

	internal class ClipboardItem : IClipboardItem, IGalleryItem
	{
		private readonly object _item;
		private readonly Image _image;
		private readonly string _description;
		private readonly Rectangle _displayRectangle;
		private int _lockCount;

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

		public void Lock()
		{
			Interlocked.Increment(ref _lockCount);
		}

		public void Unlock()
		{
			Interlocked.Decrement(ref _lockCount);
		}

		public bool Locked
		{
			get { return Thread.VolatileRead(ref _lockCount) != 0; }
		}
	}
}
