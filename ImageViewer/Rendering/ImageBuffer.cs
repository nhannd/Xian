#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal class ImageBuffer : IDisposable
    {
		private System.Drawing.Graphics _graphics;
		private Bitmap _buffer;
		private Size _size;
		private readonly bool _useIndexedBuffer;

        public ImageBuffer(bool useIndexedBuffer)
        {
        	_useIndexedBuffer = useIndexedBuffer;
        }

		public System.Drawing.Graphics Graphics
		{
			get
			{
				if (_graphics == null)
					_graphics = System.Drawing.Graphics.FromImage(Bitmap);

				return _graphics;
			}
		}

		public Bitmap Bitmap
		{
			get
			{
				CreateBuffer();
				return _buffer;
			}
		}

		public Size Size
		{
			get { return _size; }
			set { _size = value; }
		}

		public void Render(ImageBuffer source)
		{
			CreateBuffer();

			if (_useIndexedBuffer)
				RenderToIndexedBuffer(source.Bitmap);
			else
				Graphics.DrawImageUnscaled(source.Bitmap, 0, 0);
		}

		private void RenderToIndexedBuffer(Bitmap source)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			_size = source.Size;

			Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
			BitmapData src = source.LockBits(rect, ImageLockMode.ReadOnly, source.PixelFormat);
			BitmapData dest = _buffer.LockBits(rect, ImageLockMode.WriteOnly, _buffer.PixelFormat);

			try
			{
				Blit(src, dest);
			}
			finally
			{
				source.UnlockBits(src);
				_buffer.UnlockBits(dest);
			}

			clock.Stop();
			RenderPerformanceReportBroker.PublishPerformanceReport("BackBuffer.RenderToIndexedImage", clock.Seconds);
		}

		private void CreateBuffer()
		{
			if (_buffer == null || _buffer.Width != _size.Width || _buffer.Height != _size.Height)
			{
				DisposeBuffer();
				if (_useIndexedBuffer)
				{
					_buffer = new Bitmap(_size.Width, _size.Height, PixelFormat.Format8bppIndexed);

					ColorPalette pal = _buffer.Palette;
					for (int i = 0; i < 256; i++)
						pal.Entries[i] = Color.FromArgb(255, i, i, i);
					_buffer.Palette = pal;
				}
				else
				{
					_buffer = new Bitmap(_size.Width, _size.Height);
				}
			}
		}

		private static unsafe void Blit(BitmapData source, BitmapData dest)
		{
			int srcOffset = source.Stride - 4 * source.Width;
			int dstOffset = dest.Stride - dest.Width;
			byte* ptrSrc = (byte*)source.Scan0;
			byte* ptrDst = (byte*)dest.Scan0;
			for (int y = 0; y < source.Height; y++, ptrSrc += srcOffset, ptrDst += dstOffset)
			{
				for (int x = 0; x < source.Width; x++, ptrSrc += 4, ptrDst++)
				{
					*ptrDst = *ptrSrc;
				}
			}
		}

		protected virtual void DisposeBuffer()
		{
			if (_graphics != null)
			{
				_graphics.Dispose();
				_graphics = null;
			}

			if (_buffer != null)
			{
				_buffer.Dispose();
				_buffer = null;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeBuffer();
			}
		}

		public void Dispose()
        {
			try
			{
				Dispose(true);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
        }
	}
}
