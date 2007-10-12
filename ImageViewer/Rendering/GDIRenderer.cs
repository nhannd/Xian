#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// A factory for <see cref="GdiRenderer"/>s.
	/// </summary>
	public sealed class GdiRendererFactory : RendererFactoryBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public GdiRendererFactory()
		{
		}

		/// <summary>
		/// Allocates a new <see cref="GdiRenderer"/>.
		/// </summary>
		protected override RendererBase GetNewRenderer()
		{
			return new GdiRenderer();
		}
	}

	/// <summary>
	/// A 2D Renderer that uses GDI.
	/// </summary>
	public class GdiRenderer : RendererBase
	{
#pragma warning disable 1591

		protected static readonly ushort MinimumFontSizeInPixels = 4;

#pragma warning restore 1591

		private Pen _pen;
		private SolidBrush _brush;
		private bool _firstImage;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public GdiRenderer()
			: base()
		{
			_pen = new Pen(Color.White);
			_brush = new SolidBrush(Color.Black);
		}

		/// <summary>
		/// Finalizer.  This should never get called, it is a fail-safe.
		/// </summary>
		~GdiRenderer()
		{
			Dispose(false);
		}

		private new GdiRenderingSurface Surface
		{
			get { return (GdiRenderingSurface) base.Surface; }	
		}

		#region Disposal

		/// <summary>
		/// Dispose method.  Inheritors should override this method to do any additional cleanup.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (_pen != null)
			{
				_pen.Dispose();
				_pen = null;
			}
			if (_brush != null)
			{
				_brush.Dispose();
				_brush = null;
			}
		}

		#endregion

		/// <summary>
		/// Initializes the member variables before calling <see cref="RendererBase.Render"/> or <see cref="RendererBase.Refresh"/>.
		/// </summary>
		protected override void Initialize(DrawArgs drawArgs)
		{
			base.Initialize(drawArgs);
			_firstImage = true;
		}

		/// <summary>
		/// Traverses and renders the scene graph.  
		/// </summary>
		protected override void Render()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			Surface.FinalBuffer.Graphics.Clear(Color.Black);
			base.Render();

			clock.Stop();
			RenderPerformanceReportBroker.PublishPerformanceReport("GDIRenderer.Render", clock.Seconds);
		}

		/// <summary>
		/// Called when <see cref="DrawArgs.DrawMode"/> is equal to <b>DrawMode.Refresh</b>.
		/// </summary>
		protected override void Refresh()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			if (Surface.FinalBuffer != null)
			{
				if (Surface.ClipRectangle.Width != 0 && Surface.ClipRectangle.Height != 0)
				{
					System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHdc(Surface.ContextID);
					Surface.FinalBuffer.RenderTo(graphics, Surface.ClipRectangle);
					graphics.Dispose();
				}
			}

			clock.Stop();
			RenderPerformanceReportBroker.PublishPerformanceReport("GDIRenderer.Refresh", clock.Seconds);
		}

		/// <summary>
		/// Draws an <see cref="ImageGraphic"/>.
		/// </summary>
		protected override void DrawImageGraphic(ImageGraphic imageGraphic)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			const int bytesPerPixel = 4;

			if (_firstImage)
			{
				// If we're rendering the first image in the scene graph, 
				// render it directly to the final buffer, since there's no
				// alpha compositing to worry about.  95% of the time, there will only
				// be one image in the scene graph, so optimize for that case
				BitmapData bitmapData = Surface.FinalBuffer.Bitmap.LockBits(
					new Rectangle(0, 0, Surface.FinalBuffer.Bitmap.Width, Surface.FinalBuffer.Bitmap.Height),
					ImageLockMode.ReadWrite,
					Surface.FinalBuffer.Bitmap.PixelFormat);

				try
				{
					ImageRenderer.Render(imageGraphic, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, Surface.ClientRectangle);
				}
				finally
				{
					_firstImage = false;
					Surface.FinalBuffer.Bitmap.UnlockBits(bitmapData);
				}
			}
			else
			{
				// If we're rendering a subsequent image in the scene graph, we render
				// to a temporary buffer first, then we render that buffer to the final
				// buffer to produce the desired compositing result.  Not very efficient
				// approach, since ideally, we'd do the compositing right on the final
				// buffer, but this will do for now.
				Bitmap bmp = new Bitmap(Surface.FinalBuffer.Width, Surface.FinalBuffer.Height, PixelFormat.Format32bppArgb);

				BitmapData bitmapData = bmp.LockBits(
					new Rectangle(0, 0, bmp.Width, bmp.Height),
					ImageLockMode.ReadWrite,
					bmp.PixelFormat);

				try
				{
					// Render to temporary buffer
					ImageRenderer.Render(imageGraphic, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, Surface.ClientRectangle);
				}
				finally
				{
					bmp.UnlockBits(bitmapData);
				}

				// Perform compositing by rendering temporary buffer to final buffer
				System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(Surface.FinalBuffer.Bitmap);
				g.DrawImage(bmp, Surface.ClientRectangle, Surface.ClientRectangle, GraphicsUnit.Pixel);
			}

			clock.Stop();
			RenderPerformanceReportBroker.PublishPerformanceReport("GDIRenderer.DrawImageGraphic", clock.Seconds);
		}

		/// <summary>
		/// Draws a <see cref="LinePrimitive"/>.
		/// </summary>
		protected override void DrawLinePrimitive(LinePrimitive line)
		{
			Surface.FinalBuffer.Graphics.Transform = line.SpatialTransform.CumulativeTransform;
			line.CoordinateSystem = CoordinateSystem.Source;

			Surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(line, 1);

			SetDashStyle(line);
#if MONO
			Size del = new Size((int)GDIRenderer._dropShadowOffset.Width, (int)GDIRenderer._dropShadowOffset.Height);

			Surface.FinalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1 + del,
				line.Pt2 + del);
#else
			Surface.FinalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1 + GetDropShadowOffset(line),
				line.Pt2 + GetDropShadowOffset(line));
#endif

			// Draw line
			_pen.Color = line.Color;

			Surface.FinalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1,
				line.Pt2);

			Surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.None;

			line.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws a <see cref="RectanglePrimitive"/>.
		/// </summary>
		protected override void DrawRectanglePrimitive(RectanglePrimitive rect)
		{
			Surface.FinalBuffer.Graphics.Transform = rect.SpatialTransform.CumulativeTransform;
			rect.CoordinateSystem = CoordinateSystem.Source;

			float offsetX = 0;
			float offsetY = 0;

			if (rect.Width < 0)
				offsetX = rect.Width;

			if (rect.Height < 0)
				offsetY = rect.Height;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(rect, 1);

			SetDashStyle(rect);

			Surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX + GetDropShadowOffset(rect).Width,
				rect.TopLeft.Y + offsetY + GetDropShadowOffset(rect).Height,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			// Draw rectangle
			_pen.Color = rect.Color;

			Surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX,
				rect.TopLeft.Y + offsetY,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			rect.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws a <see cref="PointPrimitive"/>.
		/// </summary>
		protected override void DrawPointPrimitive(PointPrimitive pointPrimitive)
		{
			Surface.FinalBuffer.Graphics.Transform = pointPrimitive.SpatialTransform.CumulativeTransform;
			pointPrimitive.CoordinateSystem = CoordinateSystem.Source;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = 1;

			SetDashStyle(pointPrimitive);

			// Draw rectangle
			_pen.Color = pointPrimitive.Color;

			Surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				pointPrimitive.Point.X,
				pointPrimitive.Point.Y,
				1,
				1);

			pointPrimitive.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws an <see cref="InvariantRectanglePrimitive"/>.
		/// </summary>
		protected override void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rect)
		{
			Surface.FinalBuffer.Graphics.Transform = rect.SpatialTransform.CumulativeTransform;
			rect.CoordinateSystem = CoordinateSystem.Source;

			float offsetX = 0;
			float offsetY = 0;

			if (rect.Width < 0)
				offsetX = rect.Width;

			if (rect.Height < 0)
				offsetY = rect.Height;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(rect, 1);

			SetDashStyle(rect);

			Surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX + GetDropShadowOffset(rect).Width,
				rect.TopLeft.Y + offsetY + GetDropShadowOffset(rect).Height,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			// Draw rectangle
			_pen.Color = rect.Color;

			Surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX,
				rect.TopLeft.Y + offsetY,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			rect.ResetCoordinateSystem();
			Surface.FinalBuffer.Graphics.ResetTransform();
		}

		/// <summary>
		/// Draws an <see cref="InvariantTextPrimitive"/>.
		/// </summary>
		protected override void DrawTextPrimitive(InvariantTextPrimitive textPrimitive)
		{
			textPrimitive.CoordinateSystem = CoordinateSystem.Destination;

			// We adjust the font size depending on the scale so that it's the same size
			// irrespective of the zoom
			Font font = new Font(textPrimitive.Font, textPrimitive.SizeInPoints);

			// Calculate how big the text will be so we can set the bounding box
			Font tempFont = new Font(textPrimitive.Font, textPrimitive.SizeInPoints);

			textPrimitive.Dimensions = Surface.FinalBuffer.Graphics.MeasureString(textPrimitive.Text, font);

			tempFont.Dispose();

			// Draw drop shadow
			_brush.Color = Color.Black;

			SizeF dropShadowOffset = new SizeF(1, 1);
#if MONO
			Size del = new Size((int)GDIRenderer._dropShadowOffset.Width, (int)GDIRenderer._dropShadowOffset.Height);

			Surface.FinalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				textPrimitive.AnchorPoint + del);
#else
			PointF boundingBoxTopLeft = new PointF(textPrimitive.BoundingBox.Left, textPrimitive.BoundingBox.Top);

			Surface.FinalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				boundingBoxTopLeft + dropShadowOffset);
#endif
			// Draw text
			_brush.Color = textPrimitive.Color;

			Surface.FinalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				boundingBoxTopLeft);

			font.Dispose();

			textPrimitive.ResetCoordinateSystem();
		}

		/// <summary>
		/// Draws an <see cref="AnnotationBox"/>.
		/// </summary>
		protected override void DrawAnnotationBox(string annotationText, AnnotationBox annotationBox)
		{
			ClientArea clientArea = new ClientArea();
			clientArea.ParentRectangle = Surface.ClientRectangle;
			clientArea.NormalizedRectangle = annotationBox.NormalizedRectangle;
			Rectangle clientRectangle = clientArea.ClientRectangle;

			//Deflate the client rectangle by 4 pixels to allow some space 
			//between neighbouring rectangles whose borders coincide.
			Rectangle.Inflate(clientRectangle, -4, -4);

			int fontSize = (clientRectangle.Height / annotationBox.NumberOfLines) - 1;

			//don't draw it if it's too small to read, anyway.
			if (fontSize < MinimumFontSizeInPixels)
				return;

			StringFormat format = new StringFormat();

			if (annotationBox.Truncation == AnnotationBox.TruncationBehaviour.Truncate)
				format.Trimming = StringTrimming.Character;
			else
				format.Trimming = StringTrimming.EllipsisCharacter;

			if (annotationBox.FitWidth)
				format.Trimming = StringTrimming.None;

			if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.Far)
				format.Alignment = StringAlignment.Far;
			else if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.Center)
				format.Alignment = StringAlignment.Center;
			else
				format.Alignment = StringAlignment.Near;

			if (annotationBox.VerticalAlignment == AnnotationBox.VerticalAlignmentBehaviour.Top)
				format.LineAlignment = StringAlignment.Near;
			else if (annotationBox.VerticalAlignment == AnnotationBox.VerticalAlignmentBehaviour.Center)
				format.LineAlignment = StringAlignment.Center;
			else
				format.LineAlignment = StringAlignment.Far;

			//allow p's and q's, etc to extend slightly beyond the bounding rectangle.  Only completely visible lines are shown.
			format.FormatFlags = StringFormatFlags.NoClip;

			if (annotationBox.NumberOfLines == 1)
				format.FormatFlags |= StringFormatFlags.NoWrap;

			FontStyle style = FontStyle.Regular;
			if (annotationBox.Bold)
				style |= FontStyle.Bold;
			if (annotationBox.Italics)
				style |= FontStyle.Italic;

			//don't draw it if it's too small to read, anyway.
			if (fontSize < MinimumFontSizeInPixels)
				return;

			Font font;
			try
			{
				font = new Font(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				font = new Font(AnnotationBox.DefaultFont, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
			}

			SizeF layoutArea = new SizeF(clientRectangle.Width, clientRectangle.Height);
			SizeF size = Surface.FinalBuffer.Graphics.MeasureString(annotationText, font, layoutArea, format);
			if (annotationBox.FitWidth && size.Width > clientRectangle.Width)
			{
				fontSize = (int)(Math.Round(fontSize * clientRectangle.Width / (double)size.Width - 0.5));

				//don't draw it if it's too small to read, anyway.
				if (fontSize < MinimumFontSizeInPixels)
					return;

				try
				{
					font = new Font(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					font = new Font(AnnotationBox.DefaultFont, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
				}
			}

			// Draw drop shadow
			_brush.Color = Color.Black;
			clientRectangle.Offset(1, 1);

			Surface.FinalBuffer.Graphics.DrawString(
				annotationText,
				font,
				_brush,
				clientRectangle,
				format);

			_brush.Color = Color.FromName(annotationBox.Color);
			clientRectangle.Offset(-1, -1);

			Surface.FinalBuffer.Graphics.DrawString(
				annotationText,
				font,
				_brush,
				clientRectangle,
				format);

			font.Dispose();
		}

		/// <summary>
		/// Draws an error message in the Scene Graph's client area of the screen.
		/// </summary>
		protected override void ShowErrorMessage(string message)
		{
			Font font = new Font("Arial", 12.0f);
			SizeF size = Surface.FinalBuffer.Graphics.MeasureString(message, font);

			int centerHeight = Surface.FinalBuffer.Height / 2;
			int centerWidth = Surface.FinalBuffer.Width / 2;

			Surface.FinalBuffer.Graphics.DrawString(
				message,
				font,
				_brush,
				new PointF(centerWidth - size.Width / 2, centerHeight - size.Height / 2));

			font.Dispose();
		}

		private void SetDashStyle(VectorGraphic graphic)
		{
			if (graphic.LineStyle == LineStyle.Solid)
			{
				_pen.DashStyle = DashStyle.Solid;
			}
			else
			{
				_pen.DashStyle = DashStyle.Custom;

				if (graphic.LineStyle == LineStyle.Dash)
					_pen.DashPattern = new float[] { 4.0F, 4.0F };
				else
					_pen.DashPattern = new float[] { 2.0F, 4.0F };
			}
		}

		private static float CalculateScaledPenWidth(IGraphic graphic, int penWidth)
		{
			return penWidth / graphic.SpatialTransform.CumulativeScale;
		}

		private static SizeF GetDropShadowOffset(IGraphic graphic)
		{
			float offset = CalculateScaledPenWidth(graphic, 1);
			return new SizeF(offset, offset);
		}

		/// <summary>
		/// Factory method for an <see cref="IRenderingSurface"/>.
		/// </summary>
		public sealed override IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
		{
			return new GdiRenderingSurface(windowID, width, height);
		}
	}
}
