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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;

namespace MyPlugin.Imaging
{
	// To enable this custom renderer, change the Enabled flag to True.
	// i.e. [ExtensionOf(typeof (BasicPresentationImageRendererFactoryExtensionPoint), Enabled = true)]
	[ExtensionOf(typeof (BasicPresentationImageRendererFactoryExtensionPoint), Enabled = false)]
	public class MyRendererFactory : RendererFactoryBase
	{
		protected override RendererBase GetNewRenderer()
		{
			return new MyRenderer();
		}
	}

	public class MyRenderer : RendererBase
	{
		public override IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
		{
			return new MyRenderingSurface(windowID, width, height);
		}

		protected override void Refresh()
		{
			// "Flip" the surface to the actual WinForms control
			MyRenderingSurface surface = base.Surface as MyRenderingSurface;

			// Get the Graphics from the HDC and draw the back buffer image to it.
			Graphics graphics = Graphics.FromHdc(surface.ContextID);
			graphics.DrawImage(surface.Bitmap, 0, 0);
			graphics.Dispose();
		}

		protected override void DrawImageGraphic(ImageGraphic imageGraphic)
		{
			// TODO: Implement this
		}

		protected override void DrawLinePrimitive(LinePrimitive line)
		{
			// TODO: Implement this
		}

		protected override void DrawInvariantLinePrimitive(InvariantLinePrimitive line)
		{
			// TODO: Implement this
		}

		protected override void DrawCurvePrimitive(CurvePrimitive curve)
		{
			// TODO: Implement this
		}

		protected override void DrawRectanglePrimitive(RectanglePrimitive rectangle)
		{
			// TODO: Implement this
		}

		protected override void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rectangle)
		{
			// TODO: Implement this
		}

		protected override void DrawEllipsePrimitive(EllipsePrimitive ellipse)
		{
			// TODO: Implement this
		}

		protected override void DrawInvariantEllipsePrimitive(InvariantEllipsePrimitive ellipse)
		{
			// TODO: Implement this
		}

		protected override void DrawArcPrimitive(IArcGraphic arc)
		{
			// TODO: Implement this
		}

		protected override void DrawPointPrimitive(PointPrimitive pointPrimitive)
		{
			// TODO: Implement this
		}

		protected override void DrawTextPrimitive(InvariantTextPrimitive textPrimitive)
		{
			// TODO: Implement this
		}

		protected override void DrawAnnotationBox(string annotationText, AnnotationBox annotationBox)
		{
			// TODO: Implement this
		}

		protected override void ShowErrorMessage(string message)
		{
			// TODO: Implement this
		}
	}
}