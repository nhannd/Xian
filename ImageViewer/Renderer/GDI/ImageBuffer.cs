using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClearCanvas.Workstation.Renderer.GDI
{
    public class ImageBuffer : IDisposable
    {
        protected Graphics _graphics;
        protected Bitmap _bitmap;

        public ImageBuffer(int width, int height)
        {
            Initialize(width, height);
        }

        protected virtual void Initialize(int width, int height)
        {
            _bitmap = new Bitmap(width, height);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            _graphics.Clear(Color.Black);
        }

        public virtual void RenderTo(Graphics dest, Rectangle rect)
        {
            dest.DrawImage(_bitmap, rect, rect, GraphicsUnit.Pixel);
        }

        public Graphics Graphics
        {
            get { return _graphics; }
        }

		public Bitmap Bitmap
		{
			get { return _bitmap; }
		}

        public int Height
        {
            get { return _bitmap.Height; }
        }

        public int Width
        {
            get { return _bitmap.Width; }
        }

        public virtual void Dispose()
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
