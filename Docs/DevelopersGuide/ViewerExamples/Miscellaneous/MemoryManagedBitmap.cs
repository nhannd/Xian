#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ClearCanvas.ImageViewer.Common;

namespace MyPlugin.Miscellaneous
{
	public class MemoryManagedBitmap : IDisposable, ILargeObjectContainer
	{
		private readonly LargeObjectContainerData _largeObjectData = new LargeObjectContainerData(Guid.NewGuid());
		private readonly object _syncLock = new object();
		private readonly string _filename;
		private volatile byte[] _pixelData;

		public MemoryManagedBitmap(string filename)
		{
			_filename = filename;
		}

		public void Dispose()
		{
			// force an unload if we're being disposed
			this.Unload();
		}

		public byte[] PixelData
		{
			get
			{
				// update the last access time
				_largeObjectData.UpdateLastAccessTime();

				// if the data is already available without blocking, return it immediately
				byte[] pixelData = _pixelData;
				if (pixelData != null)
					return pixelData;

				// wait for synchronized access
				lock (_syncLock)
				{
					// if the data is now available, return it immediately
					// (i.e. we were blocked because we were already reading the data)
					if (_pixelData != null)
						return _pixelData;

					// read the bitmap into memory
					using (Bitmap bmp = new Bitmap(_filename))
					{
						BitmapData bmpData = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
						try
						{
							_pixelData = new byte[bmpData.Stride*bmpData.Height];
							Marshal.Copy(bmpData.Scan0, _pixelData, 0, _pixelData.Length);
						}
						finally
						{
							bmp.UnlockBits(bmpData);
						}
					}

					// update our stats
					_largeObjectData.BytesHeldCount = _pixelData.Length;
					_largeObjectData.LargeObjectCount = 1;

					// regenerating the pixel data is easy when it's stored on the hard drive!
					_largeObjectData.RegenerationCost = RegenerationCost.Low;

					// register with memory manager
					MemoryManager.Add(this);

					return _pixelData;
				}
			}
		}

		public void Unload()
		{
			// wait for synchronized access
			lock (_syncLock)
			{
				// dump our data
				_pixelData = null;

				// update our stats
				_largeObjectData.BytesHeldCount = 0;
				_largeObjectData.LargeObjectCount = 0;

				// unregister with memory manager
				MemoryManager.Remove(this);
			}
		}

		#region ILargeObjectContainer Members

		Guid ILargeObjectContainer.Identifier
		{
			get { return _largeObjectData.Identifier; }
		}

		int ILargeObjectContainer.LargeObjectCount
		{
			get { return _largeObjectData.LargeObjectCount; }
		}

		long ILargeObjectContainer.BytesHeldCount
		{
			get { return _largeObjectData.BytesHeldCount; }
		}

		DateTime ILargeObjectContainer.LastAccessTime
		{
			get { return _largeObjectData.LastAccessTime; }
		}

		RegenerationCost ILargeObjectContainer.RegenerationCost
		{
			get { return _largeObjectData.RegenerationCost; }
		}

		bool ILargeObjectContainer.IsLocked
		{
			get { return _largeObjectData.IsLocked; }
		}

		void ILargeObjectContainer.Lock()
		{
			_largeObjectData.Lock();
		}

		void ILargeObjectContainer.Unlock()
		{
			_largeObjectData.Unlock();
		}

		#endregion
	}
}