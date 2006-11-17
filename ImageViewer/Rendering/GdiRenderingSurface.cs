using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Rendering
{
	public class GdiRenderingSurface : IRenderingSurface
	{
		private ImageBuffer _imageBuffer;
		private ImageBuffer _finalBuffer;
		private IntPtr _windowID;
		private IntPtr _contextID;

		public GdiRenderingSurface(IntPtr windowID, int width, int height)
		{
			if (width == 0 || height == 0)
				return;

			_imageBuffer = new ImageBuffer(width, height);
#if MONO
			_finalBuffer = new ImageBuffer(width, height);
#else
			//_finalBuffer = new ImageBufferWin32(width, height);
			_finalBuffer = new ImageBuffer(width, height);
#endif
			_windowID = windowID;
		}

		#region IRenderingSurface Members

		public IntPtr WindowID
		{
			get { return _windowID; }
			set { _windowID = value; }
		}

		public IntPtr ContextID 
		{
			get { return _contextID; }
			set { _contextID = value; }
		}

		#endregion

		public ImageBuffer ImageBuffer
		{
			get { return _imageBuffer; }
		}

		public ImageBuffer FinalBuffer
		{
			get { return _finalBuffer; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeOffscreenBuffers();
			}
		}

		private void DisposeOffscreenBuffers()
		{
			if (_imageBuffer != null)
			{
				_imageBuffer.Dispose();
				_imageBuffer = null;
			}
			if (_finalBuffer != null)
			{
				_finalBuffer.Dispose();
				_finalBuffer = null;
			}
		}
	}
}
