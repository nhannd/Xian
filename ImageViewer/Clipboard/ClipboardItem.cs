using System;
using System.Drawing;
using System.Threading;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Clipboard
{
	/// <summary>
	/// Defines an item that can be stored in the clipboard.
	/// </summary>
	public interface IClipboardItem
	{
		/// <summary>
		/// Returns the actual clipboard item.
		/// </summary>
		object Item { get; }

		/// <summary>
		/// Locks a clipboard item.
		/// </summary>
		/// <remarks>
		/// This method should be called when an asynchronous operation is about to
		/// be performed with this <see cref="IClipboardItem"/>.  Locking prevents
		/// removal of the <see cref="IClipboardItem"/> from the clipboard, which
		/// would result in <see cref="Item"/>'s disposal.
		/// </remarks>
		void Lock();

		/// <summary>
		/// Unlocks a clipboard item.
		/// </summary>
		/// <remarks>
		/// <remarks>
		/// This method should be called when an asynchronous operation, performed 
		/// with this <see cref="IClipboardItem"/>, has completed.  Locking prevents
		/// removal of the <see cref="IClipboardItem"/> from the clipboard, which
		/// would result in <see cref="Item"/>'s disposal.
		/// </remarks>
		/// </remarks>
		void Unlock();
	}

	internal class ClipboardItem : IClipboardItem, IGalleryItem, IDisposable
	{
		private object _item;
		private Image _image;
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

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			if (_item != null && _item is IDisposable)
			{
				((IDisposable)_item).Dispose();
				_item = null;
			}
			if (_image != null)
			{
				_image.Dispose();
				_image = null;
			}
		}

		#endregion
	}
}
