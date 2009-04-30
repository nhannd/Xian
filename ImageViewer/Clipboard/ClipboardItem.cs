#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
		private readonly string _name;
		private readonly Rectangle _displayRectangle;
		private int _lockCount;

		public ClipboardItem(object item, Image image, string description, Rectangle displayRectangle)
		{
			_item = item;
			_image = image;
			_name = description;
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

		public string Name
		{
			get { return _name; }
			set { throw new NotImplementedException("Renaming clipboard items is not yet supported."); }
		}

		public string Description
		{
			get { return string.Empty; }
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
				((IDisposable) _item).Dispose();
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