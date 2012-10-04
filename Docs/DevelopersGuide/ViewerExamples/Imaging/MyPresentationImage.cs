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
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Rendering;

namespace MyPlugin.Imaging
{
	public class MyPresentationImage : PresentationImage
	{
		public MyPresentationImage() {}

		public override IRenderer ImageRenderer
		{
			get
			{
				if (base.ImageRenderer == null)
					base.ImageRenderer = new MyPresentationImageRenderer();

				return base.ImageRenderer;
			}
		}

		public override IPresentationImage CreateFreshCopy()
		{
			// TODO: implement a copy method
			throw new Exception("The method or operation is not implemented.");
		}
	}

	public class MyPresentationImageRenderer : IRenderer
	{
		public MyPresentationImageRenderer() {}

		#region IRenderer members

		public IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
		{
			return new MyRenderingSurface(windowID, width, height);
		}

		public void Draw(DrawArgs drawArgs)
		{
			if (drawArgs.DrawMode == DrawMode.Render)
				Render(drawArgs);
			else
				Refresh(drawArgs);
		}

		#endregion

		private void Render(DrawArgs drawArgs)
		{
			MyRenderingSurface surface = drawArgs.RenderingSurface as MyRenderingSurface;

			// Recursively render the nodes in the scene graph to the surface here   
		}

		private void Refresh(DrawArgs drawArgs)
		{
			// "Flip" the surface to the actual WinForms control
			MyRenderingSurface surface = drawArgs.RenderingSurface as MyRenderingSurface;

			// Get the Graphics from the HDC and draw the back buffer image to it.
			Graphics graphics = Graphics.FromHdc(surface.ContextID);
			graphics.DrawImage(surface.Bitmap, 0, 0);
			graphics.Dispose();
		}

		#region IDisposable Members

		public void Dispose()
		{
			// dispose of any remaining unmanaged resources here
		}

		#endregion
	}

	public class MyRenderingSurface : IRenderingSurface
	{
		private IntPtr _windowID;
		private IntPtr _contextID;
		private Bitmap _bitmap;
		private Rectangle _clientRectangle;
		private Rectangle _clipRectangle;

		public MyRenderingSurface(IntPtr windowID, int width, int height)
		{
			_windowID = windowID;
			_bitmap = new Bitmap(width, height);
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

		// The offscreen buffer we'll use to render our scene graph to
		public Bitmap Bitmap
		{
			get { return _bitmap; }
		}

		#region IRenderingSurface Members

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set { _clientRectangle = value; }
		}

		public Rectangle ClipRectangle
		{
			get { return _clipRectangle; }
			set { _clipRectangle = value; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			// dispose of any remaining unmanaged resources here
			_bitmap.Dispose();
		}

		#endregion
	}
}