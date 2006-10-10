using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Rendering
{
	public class DicomPresentationImageRenderer : IRenderer, IDisposable
	{
		#region Private Fields

		private static readonly SizeF _dropShadowOffset = new SizeF(1, 1);
		private static readonly ushort _minimumFontSizeInPixels = 4;

		private Pen _pen = new Pen(Color.White);
		private SolidBrush _brush = new SolidBrush(Color.White);
		private GdiRenderingSurface _surface;

		#endregion

		public DicomPresentationImageRenderer()
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

			_pen = new Pen(Color.White);

			DrawLayerGroup(
				drawArgs.PresentationImage.LayerManager.RootLayerGroup, 
				drawArgs.ClientRectangle, 
				true);

			DrawTextOverlay(drawArgs.PresentationImage, drawArgs.ClientRectangle, (drawArgs.Tile as Tile).AnnotationBoxes);

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
					Graphics graphics = Graphics.FromHdc(_surface.ContextID);
					_surface.FinalBuffer.RenderTo(graphics, drawArgs.ClipRectangle);
                }
			}

			clock.Stop();
			string str = String.Format("Refresh: {0}\n", clock.ToString());
			Trace.Write(str);
		}

		private void DrawLayerGroup(
			LayerGroup layerGroup, 
			Rectangle drawableTileRectangle, 
			bool fastDraw)
		{
			// If the tile has no PresentationImage associated with it (and thus no
			// LayerGroup), just leave it blank.
			if (layerGroup == null)
			{
				_surface.FinalBuffer.Graphics.Clear(Color.Black);
				return;
			}

			foreach (Layer layer in layerGroup.Layers)
			{
				if (layer is LayerGroup)
				{
					DrawLayerGroup((LayerGroup)layer, drawableTileRectangle, fastDraw);
				}
				else
				{
					if (layer is ImageLayer)
					{
						_surface.FinalBuffer.Graphics.Clear(Color.Black);

						if (layer is ImageLayer)
						{
							ImageLayer imageLayer = (ImageLayer)layer;
							imageLayer.FastRender = true;
							DrawImageLayer(imageLayer, drawableTileRectangle);
						}
					}

					if (layer is GraphicLayer)
					{
						_surface.FinalBuffer.Graphics.SetClip(drawableTileRectangle);
						DrawGraphicLayer(layer as GraphicLayer);
						_surface.FinalBuffer.Graphics.ResetClip();
					}
				}
			}
		}
	
		private void DrawImageLayer(ImageLayer layer, Rectangle clientArea)
		{
			if (layer == null)
				return;

			CodeClock counter = new CodeClock();
			counter.Start();

			BitmapData bitmapData = _surface.FinalBuffer.Bitmap.LockBits(
				new Rectangle(0, 0, _surface.FinalBuffer.Bitmap.Width, _surface.FinalBuffer.Bitmap.Height),
				ImageLockMode.ReadWrite,
				_surface.FinalBuffer.Bitmap.PixelFormat);

			int bytesPerPixel = 4;
			ImageRenderer.Render(layer, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, clientArea);

			_surface.FinalBuffer.Bitmap.UnlockBits(bitmapData);

			counter.Stop();

			string str = String.Format("DrawImageLayer: {0}\n", counter.ToString());
			Trace.Write(str);
		}

		private void DrawGraphicLayer(GraphicLayer graphicLayer)
		{
			foreach (Graphic graphic in graphicLayer.Graphics)
				DrawGraphic(graphic);
		}

		private void DrawGraphic(Graphic graphic)
		{
			if (!graphic.Visible)
				return;

			if (graphic.IsLeaf)
			{
				if (graphic is LinePrimitive)
					DrawLinePrimitive(graphic as LinePrimitive);
				else if (graphic is RectanglePrimitive)
					DrawRectanglePrimitive(graphic as RectanglePrimitive);
				else if (graphic is InvariantRectanglePrimitive)
					DrawInvariantRectanglePrimitive(graphic as InvariantRectanglePrimitive);
				else if (graphic is InvariantTextPrimitive)
					DrawTextPrimitive(graphic as InvariantTextPrimitive);
			}
			else
			{
				foreach (Graphic subGraphic in graphic.Graphics)
					DrawGraphic(subGraphic);
			}
		}

		private void DrawLinePrimitive(LinePrimitive line)
		{
			line.CoordinateSystem = CoordinateSystem.Destination;
			
			_surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			
			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = 1;

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
				line.Pt1 + DicomPresentationImageRenderer._dropShadowOffset,
				line.Pt2 + DicomPresentationImageRenderer._dropShadowOffset);
#endif

            // Draw line
			_pen.Color = line.Color;
			_pen.Width = 1;

            _surface.FinalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1,
				line.Pt2);

            _surface.FinalBuffer.Graphics.SmoothingMode = SmoothingMode.None;

			line.ResetCoordinateSystem();
		}

		private void DrawRectanglePrimitive(RectanglePrimitive rect)
		{
			rect.CoordinateSystem = CoordinateSystem.Destination;

			float offsetX = 0;
			float offsetY = 0;

			if (rect.Width < 0)
				offsetX = rect.Width;

			if (rect.Height < 0)
				offsetY = rect.Height;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = 1;

			SetDashStyle(rect);
			
			_surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX + DicomPresentationImageRenderer._dropShadowOffset.Width,
				rect.TopLeft.Y + offsetY + DicomPresentationImageRenderer._dropShadowOffset.Height,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			// Draw rectangle
			_pen.Color = rect.Color;
			_pen.Width = 1;

            _surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX,
				rect.TopLeft.Y + offsetY,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			rect.ResetCoordinateSystem();
		}

		private void DrawInvariantRectanglePrimitive(InvariantRectanglePrimitive rect)
		{
			rect.CoordinateSystem = CoordinateSystem.Destination;

			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = 1;

			SetDashStyle(rect);
			
			_surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + DicomPresentationImageRenderer._dropShadowOffset.Width,
				rect.TopLeft.Y + DicomPresentationImageRenderer._dropShadowOffset.Height,
				rect.Width,
				rect.Height);

			// Draw rectangle
			_pen.Color = rect.Color;
			_pen.Width = 1;

            _surface.FinalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X,
				rect.TopLeft.Y,
				rect.Width,
				rect.Height);

			rect.ResetCoordinateSystem();
		}

		private void DrawTextPrimitive(InvariantTextPrimitive textPrimitive)
		{
			textPrimitive.CoordinateSystem = CoordinateSystem.Destination;

			//_surface.FinalBuffer.Graphics.Transform = textPrimitive.SpatialTransform.Transform;

			Font font = new Font(textPrimitive.Font, textPrimitive.SizeInPoints);
			
			textPrimitive.Dimensions = _surface.FinalBuffer.Graphics.MeasureString(textPrimitive.Text, font);

			// Draw drop shadow
			_brush.Color = Color.Black;

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
				textPrimitive.AnchorPoint + DicomPresentationImageRenderer._dropShadowOffset);
#endif
			// Draw text
			_brush.Color = textPrimitive.Color;

			_surface.FinalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				textPrimitive.AnchorPoint);

			//_surface.FinalBuffer.Graphics.ResetTransform();

			font.Dispose();

			textPrimitive.ResetCoordinateSystem();
		}

		private void DrawTextOverlay(IPresentationImage presentationImage, Rectangle rectangle, IEnumerable<AnnotationBox> annotationBoxes)
		{
			if (annotationBoxes == null)
				return;

			foreach (AnnotationBox annotationBox in annotationBoxes)
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

			if (annotationBox.Truncation == AnnotationBox.TruncationBehaviour.TRUNCATE)
				format.Trimming = StringTrimming.Character;
			else
				format.Trimming = StringTrimming.EllipsisCharacter;

			if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.FAR)
				format.Alignment = StringAlignment.Far;
			else if (annotationBox.Justification == AnnotationBox.JustificationBehaviour.CENTRE)
				format.Alignment = StringAlignment.Center;
			else
				format.Alignment = StringAlignment.Near;

			//allow p's and q's, etc to extend slightly beyond the bounding rectangle.
			format.FormatFlags = StringFormatFlags.NoClip;

			if (annotationBox.NumberOfLines == 1)
				format.FormatFlags |= StringFormatFlags.NoWrap;

			FontStyle style = FontStyle.Regular;
			if (annotationBox.Bold)
				style |= FontStyle.Bold;
			if (annotationBox.Italics)
				style |= FontStyle.Italic;

			Font font;
			try
			{
				font = new Font(annotationBox.Font, fontSize, style, GraphicsUnit.Pixel);
			}
			catch
			{
				font = new Font(AnnotationBox.DefaultFont, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
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

		private void SetDashStyle(Graphic graphic)
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

	}
}
