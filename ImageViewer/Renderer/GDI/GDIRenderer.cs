using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Layers;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Renderer.GDI
{
	/// <summary>
	/// Summary description for GDIRenderer.
	/// </summary>
	[ExtensionOf(typeof(ClearCanvas.ImageViewer.RendererExtensionPoint))]
    public class GDIRenderer : IRenderer
	{
		private static readonly SizeF _dropShadowOffset = new SizeF(1, 1);
		private static readonly ushort _minimumFontSizeInPixels = 4;

		// Private attributes
        private ImageBuffer _imageBuffer;
        private ImageBuffer _finalBuffer;

		private Pen _pen = new Pen(Color.White);
		private SolidBrush _brush = new SolidBrush(Color.White);

		// Constructor
		public GDIRenderer()
		{
		}

		#region IDisposable Members

        public void Dispose()
		{
			DisposeOffscreenBuffers();
			_pen.Dispose();
			_brush.Dispose();
		}

		#endregion

		#region IRenderer Members

		public void Draw(Graphics graphics, ImageDrawingEventArgs e)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			if (e.PresentationImage == null)
				return;
			
			Rectangle clientRect = e.PhysicalWorkspace.ClientRectangle;
			if(clientRect.Height == 0 || clientRect.Width == 0)
				return;

			InitializeOffscreenBuffers(clientRect);
			_pen = new Pen(Color.White);

			if (e.ImageBoxLayoutChanged)
				ClearBuffer(clientRect);

			if (e.TileLayoutChanged)
				ClearBuffer(e.ImageBox.DrawableClientRectangle);

			ICustomDrawable customImage = e.PresentationImage as ICustomDrawable;

			if (customImage != null)
				DrawToScreen(customImage, graphics, e.Tile.DrawableClientRectangle);
			else
				DrawLayerGroup(e.PresentationImage.LayerManager.RootLayerGroup, e.Tile.DrawableClientRectangle, e.FastDraw);

			DrawTextOverlay(e.PresentationImage, e.Tile.DrawableClientRectangle, e.Tile.AnnotationBoxes);

			DrawImageBoxFrame(e.ImageBox.ClientRectangle, e.ImageBox.Selected, e.ImageBox.DisplaySet.Linked);

			// Only bother drawing the tile boxes if there is more than one tile in the image box
			if (e.ImageBox.Tiles.Count > 1)
				DrawTileFrame(e.Tile.DrawableClientRectangle, e.Tile.Selected);

			clock.Stop();

			string str = String.Format("Draw: {0}\n", clock.ToString());
			Trace.Write(str);
		}

		public void Paint(Graphics graphics, Rectangle repaintArea)
		{
			if (_finalBuffer != null)
			{
                if (repaintArea.Width != 0 && repaintArea.Height != 0)
                {
                    _finalBuffer.RenderTo(graphics, repaintArea);
                }
			}
		}

		#endregion

		private void InitializeOffscreenBuffers(Rectangle workspaceArea)
		{
			// Lazy initialize the offscreen bitmap
			if (_finalBuffer == null)
			{
				CreateOffscreenBuffers(workspaceArea.Width, workspaceArea.Height);
			}
			else
			{
				// If the size of the client area has changed since the last time
				// Draw was called, create a new offscreen bitmap to fit the new client area
				if (_finalBuffer.Width != workspaceArea.Width ||
                    _finalBuffer.Height != workspaceArea.Height)
				{
					DisposeOffscreenBuffers();
					CreateOffscreenBuffers(workspaceArea.Width, workspaceArea.Height);
				}
			}
		}

		private void CreateOffscreenBuffers(int width, int height)
		{
			_imageBuffer = new ImageBuffer(width, height);

#if MONO
			_finalBuffer = new ImageBuffer(width, height);
#else
			_finalBuffer = new ImageBufferWin32(width, height);
#endif
		}

//        private ImageBuffer CreateImageBuffer(int width, int height)
//        {
//            // The optimized Win32 buffer does not work with the GTK view, for unknown reasons
//            // Therefore, always use default ImageBuffer when compiling for Mono, even on Win32
//#if MONO
//            return new ImageBuffer(width, height);
//#else
//            return new ImageBufferWin32(width, height);
//#endif
//        }

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

		private void ClearBuffer(Rectangle rect)
		{
			_imageBuffer.Graphics.SetClip(rect);
            _imageBuffer.Graphics.Clear(Color.Black);
			_imageBuffer.Graphics.ResetClip();

			_finalBuffer.Graphics.SetClip(rect);
            _finalBuffer.Graphics.Clear(Color.Black);
            _finalBuffer.Graphics.ResetClip();
		}

		private void DrawToScreen(ICustomDrawable image, Graphics screenGraphics, Rectangle clientArea)
		{
			IntPtr hDC = screenGraphics.GetHdc();
			image.Draw(hDC, clientArea);
			screenGraphics.ReleaseHdc(hDC);
		}

		private void DrawLayerGroup(LayerGroup layerGroup, Rectangle drawableTileRectangle, bool fastDraw)
		{
			// If the tile has no PresentationImage associated with it (and thus no
			// LayerGroup), just leave it blank.
			if (layerGroup == null)
			{
				_finalBuffer.Graphics.SetClip(drawableTileRectangle);
                _finalBuffer.Graphics.Clear(Color.Black);
                _finalBuffer.Graphics.ResetClip();

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
					if (layer is ImageLayer ||
						layer is CustomLayer)
					{
						// When the image layer needs to be redrawn, we draw it
						// to a separate buffer.  This buffer essentially contains
						// a pre-rendered image that can at anytime be blasted to the
						// screen quickly, such as when a graphic is moved
						// around on the screen and the image underneath it needs to
						// be repainted whenever the object is moved.
						if (layer.RedrawRequired)
						{
							_imageBuffer.Graphics.SetClip(drawableTileRectangle);
							_imageBuffer.Graphics.Clear(Color.Black);
							_imageBuffer.Graphics.ResetClip();

							if (layer is ImageLayer)
							{
								ImageLayer imageLayer = (ImageLayer)layer;
								imageLayer.FastRender = fastDraw;
								DrawImageLayer(imageLayer, drawableTileRectangle);
							}

							if (layer is CustomLayer)
								DrawCustomLayer((CustomLayer)layer, drawableTileRectangle);

							layer.RedrawRequired = false;
						}

						_imageBuffer.RenderTo(_finalBuffer.Graphics, drawableTileRectangle);
					}

					if (layer is GraphicLayer)
					{
						_finalBuffer.Graphics.SetClip(drawableTileRectangle);
						DrawGraphicLayer(layer as GraphicLayer);
						_finalBuffer.Graphics.ResetClip();
					}
				}
			}
		}
	
		private void DrawImageLayer(ImageLayer layer, Rectangle clientArea)
		{
			if (layer == null)
				return;

			//CodeClock counter = new CodeClock();
			//counter.Start();

			BitmapData bitmapData = _imageBuffer.Bitmap.LockBits(
				new Rectangle(0, 0, _imageBuffer.Bitmap.Width, _imageBuffer.Bitmap.Height),
				ImageLockMode.ReadWrite,
				_imageBuffer.Bitmap.PixelFormat);

			int bytesPerPixel = 4;
			ImageRenderer.Render(layer, bitmapData.Scan0, bitmapData.Width, bytesPerPixel, clientArea);

			_imageBuffer.Bitmap.UnlockBits(bitmapData);

			//counter.Stop();

			//string str = String.Format("Render: {0}\n", counter.ToString());
			//Trace.Write(str);
		}

		private void DrawCustomLayer(CustomLayer layer, Rectangle clientArea)
		{
			if (layer == null)
				return;

			IntPtr hDC = new IntPtr();
            hDC = _imageBuffer.Graphics.GetHdc();
			layer.Draw(new DrawCustomLayerEventArgs(hDC, clientArea));
            _imageBuffer.Graphics.ReleaseHdc(hDC);
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
			
			_finalBuffer.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			
			// Draw drop shadow
			_pen.Color = Color.Black;
			_pen.Width = 1;

			SetDashStyle(line);
#if MONO
			Size del = new Size((int)GDIRenderer._dropShadowOffset.Width, (int)GDIRenderer._dropShadowOffset.Height);

            _finalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1 + del,
				line.Pt2 + del);
#else
			_finalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1 + GDIRenderer._dropShadowOffset,
				line.Pt2 + GDIRenderer._dropShadowOffset);
#endif

            // Draw line
			_pen.Color = line.Color;
			_pen.Width = 1;

            _finalBuffer.Graphics.DrawLine(
				_pen,
				line.Pt1,
				line.Pt2);

            _finalBuffer.Graphics.SmoothingMode = SmoothingMode.None;

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
			
			_finalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + offsetX + GDIRenderer._dropShadowOffset.Width,
				rect.TopLeft.Y + offsetY + GDIRenderer._dropShadowOffset.Height,
				Math.Abs(rect.Width),
				Math.Abs(rect.Height));

			// Draw rectangle
			_pen.Color = rect.Color;
			_pen.Width = 1;

            _finalBuffer.Graphics.DrawRectangle(
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
			
			_finalBuffer.Graphics.DrawRectangle(
				_pen,
				rect.TopLeft.X + GDIRenderer._dropShadowOffset.Width,
				rect.TopLeft.Y + GDIRenderer._dropShadowOffset.Height,
				rect.Width,
				rect.Height);

			// Draw rectangle
			_pen.Color = rect.Color;
			_pen.Width = 1;

            _finalBuffer.Graphics.DrawRectangle(
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

			//_finalBuffer.Graphics.Transform = textPrimitive.SpatialTransform.Transform;

			Font font = new Font(textPrimitive.Font, textPrimitive.SizeInPoints);
			
			textPrimitive.Dimensions = _finalBuffer.Graphics.MeasureString(textPrimitive.Text, font);

			// Draw drop shadow
			_brush.Color = Color.Black;

#if MONO
			Size del = new Size((int)GDIRenderer._dropShadowOffset.Width, (int)GDIRenderer._dropShadowOffset.Height);

			_finalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				textPrimitive.AnchorPoint + del);
#else
			_finalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				textPrimitive.AnchorPoint + GDIRenderer._dropShadowOffset);
#endif
			// Draw text
			_brush.Color = textPrimitive.Color;

			_finalBuffer.Graphics.DrawString(
				textPrimitive.Text,
				font,
				_brush,
				textPrimitive.AnchorPoint);

			//_finalBuffer.Graphics.ResetTransform();

			font.Dispose();

			textPrimitive.ResetCoordinateSystem();
		}

		private void DrawImageBoxFrame(Rectangle frameRectangle, bool selected, bool isLinked)
		{
			if (selected)
				_pen.Color = Color.White;
			else
				_pen.Color = Color.Gray;

			_pen.Width = 1;
			_pen.DashStyle = DashStyle.Solid;
			
			_finalBuffer.Graphics.DrawRectangle(_pen, Rectangle.Inflate(frameRectangle, -2, -2));

			if (isLinked)
			{
				_pen.Color = Color.Red;
                _finalBuffer.Graphics.DrawRectangle(_pen, Rectangle.Inflate(frameRectangle, -3, -3));
			}
			else
			{
				_pen.Color = Color.Black;
                _finalBuffer.Graphics.DrawRectangle(_pen, Rectangle.Inflate(frameRectangle, -3, -3));
			}
		}

		private void DrawTileFrame(Rectangle frameRectangle, bool selected)
		{
			if (selected)
				_pen.Color = Color.Yellow;
			else
				_pen.Color = Color.Gray;
				_pen.Width = 1;

			_pen.Width = 1;
			_pen.DashStyle = DashStyle.Solid;
			
			_finalBuffer.Graphics.DrawRectangle(_pen, frameRectangle);
		}

		private void DrawTextOverlay(PresentationImage presentationImage, Rectangle rectangle, IEnumerable<AnnotationBox> annotationBoxes)
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

			_finalBuffer.Graphics.DrawString(
				annotationText,
				font,
				_brush,
				clientRectangle,
				format);
			
			_brush.Color = Color.FromName(annotationBox.Color);
			clientRectangle.Offset(-1, -1); 
			
			_finalBuffer.Graphics.DrawString(
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
