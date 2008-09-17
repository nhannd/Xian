using System;
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal sealed class ManagedBackBuffer : BackBuffer
	{
		private BufferedGraphicsContext _graphicsContext;
		private BufferedGraphics _bufferedGraphics;

		private ImageBuffer _colorBuffer;
		private ImageBuffer _indexedBuffer;

		public ManagedBackBuffer(bool useIndexedBuffer)
		{
			_colorBuffer = new ImageBuffer(false);

			if (useIndexedBuffer)
				_indexedBuffer = new ImageBuffer(true);
		}

		public override System.Drawing.Graphics Graphics
		{
			get
			{
				if (BufferedGraphics != null)
					return BufferedGraphics.Graphics;

				return null;
			}
		}

		protected override ImageBuffer ColorBuffer
		{
			get { return _colorBuffer; }
		}

		private BufferedGraphics BufferedGraphics
		{
			get
			{
				if (_bufferedGraphics == null && !IsClientRectangleEmpty && ContextID != IntPtr.Zero)
				{
					Context.MaximumBuffer = GetMaximumBufferSize();
					_bufferedGraphics = Context.Allocate(ContextID, ClientRectangle);
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

		public override void RenderImageGraphic(ImageGraphic imageGraphic)
		{
			_colorBuffer.Graphics.Clear(Color.Black);
			base.RenderImageGraphic(imageGraphic);
			RenderColorBuffer();
		}

		private void RenderColorBuffer()
		{
			if (IsClientRectangleEmpty)
				return;

			CodeClock clock = new CodeClock();
			clock.Start();

			if (_indexedBuffer != null)
			{
				_indexedBuffer.Render(_colorBuffer);
				Graphics.DrawImageUnscaled(_indexedBuffer.Bitmap, 0, 0);
			}
			else
			{
				Graphics.DrawImageUnscaled(_colorBuffer.Bitmap, 0, 0);
			}

			clock.Stop();
			RenderPerformanceReportBroker.PublishPerformanceReport("BackBuffer.RenderImage", clock.Seconds);
		}

		public override void RenderToScreen()
		{
			if (_bufferedGraphics != null && !IsClientRectangleEmpty)
				_bufferedGraphics.Render(ContextID);
		}

		protected override void OnClientRectangleChanged()
		{
			DisposeBuffer();

			if (_colorBuffer!= null)
				_colorBuffer.Size = new Size(ClientRectangle.Width, ClientRectangle.Height);

			if (_indexedBuffer != null)
				_indexedBuffer.Size = new Size(ClientRectangle.Width, ClientRectangle.Height);
		}

		private Size GetMaximumBufferSize()
		{
			return new Size(ClientRectangle.Width + 1, ClientRectangle.Height + 1);
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

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeBuffer();

				if (_graphicsContext != null)
				{
					_graphicsContext.Dispose();
					_graphicsContext = null;
				}

				if (_colorBuffer != null)
				{
					_colorBuffer.Dispose();
					_colorBuffer = null;
				}

				if (_indexedBuffer != null)
				{
					_indexedBuffer.Dispose();
					_indexedBuffer = null;
				}
			}
		}

		#endregion
	}
}