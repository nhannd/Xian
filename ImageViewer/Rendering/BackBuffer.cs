using System;
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Rendering
{
	internal abstract class BackBuffer : IDisposable
	{
		private IntPtr _contextID;
		private Rectangle _clientRectangle;

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
				OnClientRectangleChanged();
			}
		}

		public abstract System.Drawing.Graphics Graphics { get; }

		protected abstract ImageBuffer ColorBuffer { get; }

		protected bool IsClientRectangleEmpty
		{
			get { return _clientRectangle.Width == 0 || _clientRectangle.Height == 0; }
		}

		public virtual void RenderImageGraphic(ImageGraphic graphic)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			const int bytesPerPixel = 4;

			Rectangle rect = new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
			BitmapData bitmapData = ColorBuffer.Bitmap.LockBits(rect, ImageLockMode.ReadWrite, ColorBuffer.Bitmap.PixelFormat);

			try
			{
				ImageRenderer.Render(graphic, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, rect);
			}
			finally
			{
				ColorBuffer.Bitmap.UnlockBits(bitmapData);
			}

			clock.Stop();
			RenderPerformanceReportBroker.PublishPerformanceReport("BackBuffer.RenderImageGraphic", clock.Seconds);
		}

		public abstract void RenderToScreen();

		protected virtual void OnClientRectangleChanged()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		#region IDisposable Members

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

		#endregion
	}
}
