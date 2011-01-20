#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal class ImageBuffer : IDisposable
    {
        private Bitmap _bitmap;
		private System.Drawing.Graphics _graphics;
		private Size _size;

        public ImageBuffer()
        {
        }

		public System.Drawing.Graphics Graphics
		{
			get
			{
				if (_graphics == null)
				{
					_graphics = System.Drawing.Graphics.FromImage(this.Bitmap);
					//_graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				}

				return _graphics;
			}
		}

        public Size Size
        {
            get { return _size; }
			set
			{
				if (_size == value)
					return;

				_size = value;
				DisposeBuffer();
			}
		}

		public int Width
		{
			get { return _size.Width; }	
		}

		public int Height
		{
			get { return _size.Height; }	
		}

		public Bitmap Bitmap
		{
			get
			{
				if (_bitmap == null && !_size.IsEmpty)
					_bitmap = new Bitmap(_size.Width, _size.Height);

				return _bitmap;
			}
		}

		public void Dispose()
        {
			DisposeBuffer();
        }

		private void DisposeBuffer()
		{
			if (_graphics != null)
			{
				_graphics.Flush();
				_graphics.Dispose();
				// MUST set bitmaps and graphics to null after disposal, 
				// or app will occasionally crash on exit
				_graphics = null;
			}

			if (_bitmap != null)
			{
				_bitmap.Dispose();
				_bitmap = null;
			}
		}
	}
}
