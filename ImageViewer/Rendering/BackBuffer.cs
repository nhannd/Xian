//#define USE_BITMAP

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Rendering
{
	#region BufferedGraphics Method

	internal sealed class BackBuffer : IDisposable
	{
		private IntPtr _contextID;
		private Rectangle _clientRectangle;

		private BufferedGraphicsContext _graphicsContext;
		private BufferedGraphics _bufferedGraphics;

		public BackBuffer()
		{
		}

		public IntPtr ContextID
		{
			get { return _contextID; }
			set { _contextID = value; }
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set
			{
				if (_clientRectangle == value)
					return;

				_clientRectangle = value;
				DisposeBuffer();
			}
		}

		public System.Drawing.Graphics Graphics
		{
			get
			{
				if (BufferedGraphics != null)
					return BufferedGraphics.Graphics;

				return null;
			}
		}

		private BufferedGraphics BufferedGraphics
		{
			get
			{
				if (_bufferedGraphics == null && !IsClientRectangleEmpty && _contextID != IntPtr.Zero)
				{
					Context.MaximumBuffer = GetMaximumBufferSize();
					_bufferedGraphics = Context.Allocate(_contextID, _clientRectangle);
				}

				return _bufferedGraphics;
			}
		}

		private BufferedGraphicsContext Context
		{
			get
			{
				if (_graphicsContext == null)
					_graphicsContext = new BufferedGraphicsContext();

				return _graphicsContext;
			}
		}

		private bool IsClientRectangleEmpty
		{
			get { return _clientRectangle.Width == 0 || _clientRectangle.Height == 0; }
		}

		public void RenderImage(ImageBuffer imageBuffer)
		{
			RenderImage(imageBuffer.Bitmap);
		}

		public void RenderImage(Image image)
		{
			Graphics.DrawImageUnscaled(image, 0, 0);
		}

		public void RenderToScreen()
		{
			if (_bufferedGraphics != null && !IsClientRectangleEmpty)
				_bufferedGraphics.Render(_contextID);
		}

		private Size GetMaximumBufferSize()
		{
			return new Size(_clientRectangle.Width + 1, _clientRectangle.Height + 1);
		}

		private void DisposeBuffer()
		{
			if (_graphicsContext != null)
				_graphicsContext.Invalidate();

			if (_bufferedGraphics != null)
			{
				_bufferedGraphics.Dispose();
				_bufferedGraphics = null;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			DisposeBuffer();

			if (_graphicsContext != null)
			{
				_graphicsContext.Dispose();
				_graphicsContext = null;
			}
		}

		#endregion
	}

	#endregion
}

