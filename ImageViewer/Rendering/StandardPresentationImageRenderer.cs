using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Rendering
{
	public class StandardPresentationImageRenderer : IRenderer, IDisposable
	{
		#region Private Fields

		private static readonly ushort _minimumFontSizeInPixels = 4;

		private Pen _pen = new Pen(Color.White);
		private SolidBrush _brush = new SolidBrush(Color.White);
		private GdiRenderingSurface _surface;

		#endregion

		public StandardPresentationImageRenderer()
		{

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
				_pen.Dispose();
				_brush.Dispose();
			}
		}

		#region IRenderer members

		public IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
		{
			return new GdiRenderingSurface(windowID, width, height);
		}

		public void Draw(DrawArgs drawArgs)
		{
			Platform.CheckForNullReference(drawArgs, "drawArgs");

			if (drawArgs.ClientRectangle.Width == 0 ||
				drawArgs.ClientRectangle.Height == 0)
				return;

			if (drawArgs.RenderingSurface == null)
				return;

			_surface = drawArgs.RenderingSurface as GdiRenderingSurface;

			if (_surface == null)
				return;

			if (drawArgs.DrawMode == DrawMode.Render)
				Render(drawArgs);
			else
				Refresh(drawArgs);
		}

		#endregion

		private void Render(DrawArgs drawArgs)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			_surface.FinalBuffer.Graphics.Clear(Color.Black);

			DrawSceneGraph(
				drawArgs.SceneGraph, 
				drawArgs.ClientRectangle, 
				true);

			DrawTextOverlay(drawArgs.PresentationImage, drawArgs.ClientRectangle);

			clock.Stop();
			string str = String.Format("Render: {0}\n", clock.ToString());
			Trace.Write(str);
		}

		private void Refresh(DrawArgs drawArgs)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			if (_surface.FinalBuffer != null)
			{
				if (drawArgs.ClipRectangle.Width != 0 && drawArgs.ClipRectangle.Height != 0)
                {
					System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHdc(_surface.ContextID);
					_surface.FinalBuffer.RenderTo(graphics, drawArgs.ClipRectangle);
                }
			}

			clock.Stop();
			string str = String.Format("Refresh: {0}\n", clock.ToString());
			Trace.Write(str);
		}

		private void DrawSceneGraph(
			CompositeGraphic compositeGraphic, 
			Rectangle drawableTileRectangle, 
			bool fastDraw)
		{
			foreach (Graphic graphic in compositeGraphic.Graphics)
			{
				if (graphic.Visible)
				{
					if (graphic is CompositeGraphic)
						DrawSceneGraph(graphic as CompositeGraphic, drawableTileRectangle, fastDraw);
					else if (graphic is ImageGraphic)
						DrawImageGraphic(graphic as ImageGraphic, drawableTileRectangle);
					else if (graphic is LinePrimitive)
						DrawLinePrimitive(graphic as LinePrimitive);
					else if (graphic is RectanglePrimitive)
						DrawRectanglePrimitive(graphic as RectanglePrimitive);
					else if (graphic is PointPrimitive)
						DrawPointPrimitive(graphic as PointPrimitive);
					else if (graphic is InvariantRectanglePrimitive)
						DrawInvariantRectanglePrimitive(graphic as InvariantRectanglePrimitive);
					else if (graphic is InvariantTextPrimitive)
						DrawTextPrimitive(graphic as InvariantTextPrimitive);
				}
			}
		}

		private void DrawImageGraphic(ImageGraphic imageGraphic, Rectangle clientArea)
		{
			if (imageGraphic == null)
				return;

			CodeClock counter = new CodeClock();
			counter.Start();

			BitmapData bitmapData = _surface.FinalBuffer.Bitmap.LockBits(
				new Rectangle(0, 0, _surface.FinalBuffer.Bitmap.Width, _surface.FinalBuffer.Bitmap.Height),
				ImageLockMode.ReadWrite,
				_surface.FinalBuffer.Bitmap.PixelFormat);

			int bytesPerPixel = 4;
			ImageRenderer.Render(imageGraphic, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, clientArea);

			_surface.FinalBuffer.Bitmap.UnlockBits(bitmapData);

			counter.Stop();

			string str = String.Format("DrawImageGraphic: {0}\n", counter.ToString());
			Trace.Write(str);
		}

		private void DrawLinePrimitive(LinePrimitive line)
		{
			_surface.FinalBuffer.Graphics.Transform = line.SpatialTransform.CumulativeTransform;
			line.CoordinateSystem = CoordinateSystem.Source;
			
			_surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			
			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = CalculateScaledPenWidth(line, 1);

			SetDashStyle(line);
#if MONO
			Size del = new Size((int)GDIRenderer._dropShadowOffset.Width, (int)GDIRenderer._dropShadowOffset.Height);

            _surface.FinalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1 + del,
				line.Pt2 + del);
#else
			_surface.FinalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1 + GetDropShadowOffset(line),
				line.Pt2 + GetDropShadowOffset(line));
#endif

            // Draw line
			_pen.Color = line.Color;

            _surface.FinalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1,
				line.Pt2);

            _surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.None;

			line.ResetCoordinateSystem();
			_surface.FinalBuffer.Graphics.ResetTransform();
		}

		private void DrawRectanglePrimitive(RectanglePrimitive rect)
		{
			_surface.FinalBuffer.Graphics.Transform = rect.SpatialTransform.CumulativeTransform;
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
			
			_surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX + GetDropShadowOffset(rect).Width,
				rect.TopLeft.Y + offsetY + GetDropShadowOffset(rect).Height,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			// Draw rectangle
			_pen.Color = rect.Color;

            _surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX,
				rect.TopLeft.Y + offsetY,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			rect.ResetCoordinateSystem();
			_surface.FinalBuffer.Graphics.ResetTransform();
		}

		private void DrawPointPrimitive(PointPrimitive pointPrimitive)
		{
			_surface.FinalBuffer.Graphics.Transform = pointPrimitive.SpatialTransform.CumulativeTransform;
			pointPrimitive.CoordinateSystem = CoordinateSystem.Source;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = 1;

			SetDashStyle(pointPrimitive);

			// Draw rectangle
			_pen.Color = pointPrimitive.Color;

			_surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				pointPrimitive.Point.X,
				pointPrimitive.Point.Y,
				1,
				1);

			pointPrimitive.ResetCoordinateSystem();
			_surface.FinalBuffer.Graphics.ResetTransform();
		}
		
		private void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rect)
		{
			_surface.FinalBuffer.Graphics.Transform = rect.SpatialTransform.CumulativeTransform;
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
			
			_surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX + GetDropShadowOffset(rect).Width,
				rect.TopLeft.Y + offsetY + GetDropShadowOffset(rect).Height,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			// Draw rectangle
			_pen.Color = rect.Color;

            _surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX,
				rect.TopLeft.Y + offsetY,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			rect.ResetCoordinateSystem();
			_surface.FinalBuffer.Graphics.ResetTransform();
		}

		private void DrawTextPrimitive(InvariantTextPrimitive textPrimitive)
		{
			textPrimitive.CoordinateSystem = CoordinateSystem.Destination;

			// We adjust the font size depending on the scale so that it's the same size
			// irrespective of the zoom
			Font font = new Font(textPrimitive.Font, textPrimitive.SizeInPoints);
			
			// Calculate how big the text will be so we can set the bounding box
			Font tempFont = new Font(textPrimitive.Font, textPrimitive.SizeInPoints);

			textPrimitive.Dimensions = _surface.FinalBuffer.Graphics.MeasureString(textPrimitive.Text, font);

			tempFont.Dispose();

			// Draw drop shadow
			_brush.Color = Color.Black;

			SizeF dropShadowOffset = new SizeF(1, 1);
#if MONO
			Size del = new Size((int)GDIRenderer._dropShadowOffset.Width, (int)GDIRenderer._dropShadowOffset.Height);

			_surface.FinalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				textPrimitive.AnchorPoint + del);
#else
			_surface.FinalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				textPrimitive.AnchorPoint + dropShadowOffset);
#endif
			// Draw text
			_brush.Color = textPrimitive.Color;

			_surface.FinalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				textPrimitive.AnchorPoint);

			font.Dispose();

			textPrimitive.ResetCoordinateSystem();
		}

		private void DrawTextOverlay(IPresentationImage presentationImage, Rectangle rectangle)
		{
			if (!(presentationImage is IAnnotationLayoutProvider))
				return;

			IAnnotationLayout layout = (presentationImage as IAnnotationLayoutProvider).AnnotationLayout;
			if (layout == null)
				return;

			foreach (AnnotationBox annotationBox in layout.AnnotationBoxes)
			{
				string annotationText = annotationBox.GetAnnotationText(presentationImage);
				//don't waste the CPU cycles.
				if (string.IsNullOrEmpty(annotationText))
					continue;

				DrawAnnotationBox(annotationText, rectangle, annotationBox);
			}
		}

		private void DrawAnnotationBox(string annotationText, Rectangle rectangle, AnnotationBox annotationBox)
		{
			ClientArea clientArea = new ClientArea();
			clientArea.ParentRectangle = rectangle;
			clientArea.NormalizedRectangle = annotationBox.NormalizedRectangle;
			Rectangle clientRectangle = clientArea.ClientRectangle;

			//Deflate the client rectangle by 4 pixels to allow some space 
			//between neighbouring rectangles whose borders coincide.
			Rectangle.Inflate(clientRectangle, -4, -4);

			int fontSize = (clientRectangle.Height / annotationBox.NumberOfLines) - 1;

			//don't draw it if it's too small to read, anyway.
			if (fontSize < _minimumFontSizeInPixels)
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
			if (fontSize < _minimumFontSizeInPixels)
				return;

			Font font;
			try
			{
				font = new Font(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel);
			}
			catch (Exception e)
			{
				Platform.Log(e);
				font = new Font(AnnotationBox.DefaultFont, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
			}

			SizeF layoutArea = new SizeF(clientRectangle.Width, clientRectangle.Height);
			SizeF size = _surface.FinalBuffer.Graphics.MeasureString(annotationText, font, layoutArea, format);
			if (annotationBox.FitWidth && size.Width > clientRectangle.Width)
			{
				fontSize = (int)(Math.Round(fontSize * clientRectangle.Width / (double)size.Width - 0.5));

				//don't draw it if it's too small to read, anyway.
				if (fontSize < _minimumFontSizeInPixels)
					return;

				try
				{
					font = new Font(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel);
				}
				catch (Exception e)
				{
					Platform.Log(e);
					font = new Font(AnnotationBox.DefaultFont, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
				}
			}

			// Draw drop shadow
			_brush.Color = Color.Black;
			clientRectangle.Offset(1, 1);

			_surface.FinalBuffer.Graphics.DrawString(
				annotationText,
				font,
				_brush,
				clientRectangle,
				format);

			_brush.Color = Color.FromName(annotationBox.Color);
			clientRectangle.Offset(-1, -1);

			_surface.FinalBuffer.Graphics.DrawString(
				annotationText,
				font,
				_brush,
				clientRectangle,
				format);

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

		private float CalculateScaledPenWidth(Graphic graphic, int penWidth)
		{
			return penWidth / graphic.SpatialTransform.CumulativeScale;
		}

		private SizeF GetDropShadowOffset(Graphic graphic)
		{
			float offset = CalculateScaledPenWidth(graphic, 1);
			return new SizeF(offset, offset);
		}
	}
}
