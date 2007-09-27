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
	#pragma warning disable 1591 //no-doc warnings.

	/// <summary>
	/// A Renderer that uses GDI.
	/// </summary>
	public class GDIRenderer : RendererBase
	{
		#region Internal Renderer Class

		protected class InternalGDIRenderer : InternalRenderer, IDisposable
		{
			private readonly GdiRenderingSurface _surface;
			private Pen _pen;
			private SolidBrush _brush;
			private bool _firstImage;
			
			public InternalGDIRenderer(IRenderingSurface surface, DrawMode drawMode, CompositeGraphic sceneGraph)
				: base(drawMode, sceneGraph)
			{
				Platform.CheckForNullReference(surface, "surface");
				_surface = surface as GdiRenderingSurface;
				Platform.CheckForNullReference(_surface, "_surface");

				_firstImage = true;
				_pen = new Pen(Color.White);
				_brush = new SolidBrush(Color.Black);
			}

			#region Disposal

			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
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
					Platform.Log(LogLevel.Warn, e);
				}
			}

			#endregion
			#endregion

			protected override void Render()
			{
				CodeClock clock = new CodeClock();
				clock.Start();

				_surface.FinalBuffer.Graphics.Clear(Color.Black);
				base.Render();

				clock.Stop();
				RenderPerformanceReportBroker.PublishPerformanceReport("GDIRenderer.Render", clock.Seconds);
			}

			protected override void Refresh()
			{
				CodeClock clock = new CodeClock();
				clock.Start();

				if (_surface.FinalBuffer != null)
				{
					if (_surface.ClipRectangle.Width != 0 && _surface.ClipRectangle.Height != 0)
					{
						System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHdc(_surface.ContextID);
						_surface.FinalBuffer.RenderTo(graphics, _surface.ClipRectangle);
						graphics.Dispose();
					}
				}

				clock.Stop();
				RenderPerformanceReportBroker.PublishPerformanceReport("GDIRenderer.Refresh", clock.Seconds);
			}

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
					BitmapData bitmapData = _surface.FinalBuffer.Bitmap.LockBits(
						new Rectangle(0, 0, _surface.FinalBuffer.Bitmap.Width, _surface.FinalBuffer.Bitmap.Height),
						ImageLockMode.ReadWrite,
						_surface.FinalBuffer.Bitmap.PixelFormat);

					try
					{
						ImageRenderer.Render(imageGraphic, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, _surface.ClientRectangle);
					}
					finally
					{
						_firstImage = false;
						_surface.FinalBuffer.Bitmap.UnlockBits(bitmapData);
					}
				}
				else
				{
					// If we're rendering a subsequent image in the scene graph, we render
					// to a temporary buffer first, then we render that buffer to the final
					// buffer to produce the desired compositing result.  Not very efficient
					// approach, since ideally, we'd do the compositing right on the final
					// buffer, but this will do for now.
					Bitmap bmp = new Bitmap(_surface.FinalBuffer.Width, _surface.FinalBuffer.Height, PixelFormat.Format32bppArgb);
					
					BitmapData bitmapData = bmp.LockBits(
						new Rectangle(0, 0, bmp.Width, bmp.Height),
						ImageLockMode.ReadWrite,
						bmp.PixelFormat);

					try
					{
						// Render to temporary buffer
						ImageRenderer.Render(imageGraphic, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, _surface.ClientRectangle);
					}
					finally
					{
						bmp.UnlockBits(bitmapData);
					}

					// Perform compositing by rendering temporary buffer to final buffer
					System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_surface.FinalBuffer.Bitmap);
					g.DrawImage(bmp, _surface.ClientRectangle, _surface.ClientRectangle, GraphicsUnit.Pixel);
				}

				clock.Stop();
				RenderPerformanceReportBroker.PublishPerformanceReport("GDIRenderer.DrawImageGraphic", clock.Seconds);
			}

			protected override void DrawLinePrimitive(LinePrimitive line)
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

			protected override void DrawRectanglePrimitive(RectanglePrimitive rect)
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

			protected override void DrawPointPrimitive(PointPrimitive pointPrimitive)
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

			protected override void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rect)
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

			protected override void DrawTextPrimitive(InvariantTextPrimitive textPrimitive)
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
				PointF boundingBoxTopLeft = new PointF(textPrimitive.BoundingBox.Left, textPrimitive.BoundingBox.Top);

				_surface.FinalBuffer.Graphics.DrawString(
					textPrimitive.Text,
					font,
					_brush,
					boundingBoxTopLeft + dropShadowOffset);
	#endif
				// Draw text
				_brush.Color = textPrimitive.Color;

				_surface.FinalBuffer.Graphics.DrawString(
					textPrimitive.Text,
					font,
					_brush,
					boundingBoxTopLeft);

				font.Dispose();

				textPrimitive.ResetCoordinateSystem();
			}

			protected override void DrawAnnotationBox(string annotationText, AnnotationBox annotationBox)
			{
				ClientArea clientArea = new ClientArea();
				clientArea.ParentRectangle = _surface.ClientRectangle;
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
					Platform.Log(LogLevel.Error, e);
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
						Platform.Log(LogLevel.Error, e);
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

			protected override void ShowErrorMessage(string message)
			{
				Font font = new Font("Arial", 12.0f);
				SizeF size = _surface.FinalBuffer.Graphics.MeasureString(message, font);

				int centerHeight = _surface.FinalBuffer.Height / 2;
				int centerWidth = _surface.FinalBuffer.Width / 2;
	 
				_surface.FinalBuffer.Graphics.DrawString(
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
		}
		
		#endregion

		protected static readonly ushort _minimumFontSizeInPixels = 4;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GDIRenderer()
		{
		}

		protected override InternalRenderer GetInternalRenderer(DrawArgs drawArgs)
		{
			if (drawArgs.RenderingSurface == null)
				return null;

			if (drawArgs.RenderingSurface.ClientRectangle.Width == 0 || drawArgs.RenderingSurface.ClientRectangle.Height == 0)
				return null;

			GdiRenderingSurface surface = drawArgs.RenderingSurface as GdiRenderingSurface;
			if (surface == null)
				return null;

			return new InternalGDIRenderer(surface, drawArgs.DrawMode, drawArgs.SceneGraph);
		}

		public sealed override IRenderingSurface GetRenderingSurface(IntPtr windowID, int width, int height)
		{
			return new GdiRenderingSurface(windowID, width, height);
		}
	}
}