#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Rendering
{
    internal class ImageBufferWin32 : ImageBuffer
    {
        private IntPtr _hDC;
        private IntPtr _hBitmap;

        public ImageBufferWin32(int width, int height)
        :base(width, height)
        {
        }

        protected override void Initialize(int width, int height)
        {
			if(!Platform.IsWin32Platform)
			{
				throw new NotSupportedException(SR.ExceptionImageBufferWin32NotSupported);
			}

            _bitmap = new Bitmap(width, height);
            _hDC = Win32.CreateCompatibleDC(IntPtr.Zero);
            _hBitmap = _bitmap.GetHbitmap();
            Win32.SelectObject(_hDC, _hBitmap);
			_graphics = System.Drawing.Graphics.FromHdc(_hDC);
            _graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            _graphics.Clear(Color.Black);
        }

        public override void Dispose()
        {
            base.Dispose();


            if (_hDC != IntPtr.Zero)
            {
                Win32.DeleteDC(_hDC);
                _hDC = IntPtr.Zero;
            }
            if (_hBitmap != IntPtr.Zero)
            {
                Win32.DeleteObject(_hBitmap);
                _hBitmap = IntPtr.Zero;
            }
        }

        public override void RenderTo(System.Drawing.Graphics dest, Rectangle rect)
        {
            IntPtr hdcDst = dest.GetHdc();
            IntPtr hdcSrc = _graphics.GetHdc();

            bool result = Win32.BitBlt(
                hdcDst,
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height,
                hdcSrc,
                rect.X,
                rect.Y,
                Win32.TernaryRasterOperations.SRCCOPY);

            _graphics.ReleaseHdc(hdcSrc);
            dest.ReleaseHdc(hdcDst);
        }
    }
}
