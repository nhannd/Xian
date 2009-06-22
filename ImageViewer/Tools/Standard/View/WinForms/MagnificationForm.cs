#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;
using System;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	internal partial class MagnificationForm : Form
	{
		private float _magnificationFactor;
		private PresentationImage _sourceImage;
		private PresentationImage _magnificationImage;
		private IRenderingSurface _renderingSurface;

		private Point _startPointTile;
		private Point _startPointDesktop;

		public MagnificationForm(float magnificationFactor, PresentationImage sourceImage, Point startPointTile)
		{
			InitializeComponent();

			Visible = false;
			this.DoubleBuffered = false;
			this.SetStyle(ControlStyles.DoubleBuffer, false);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

			if (Form.ActiveForm != null)
				this.Owner = Form.ActiveForm;

			MagnificationFactor = magnificationFactor;
			SourceImage = sourceImage;

			_startPointTile = startPointTile;
			_startPointDesktop = Centre = Cursor.Position;
		}

		public float MagnificationFactor
		{
			get
			{
				return _magnificationFactor;
			}	
			set
			{
				Platform.CheckTrue(value > 1.0F, "MagnificationFactor > 1");

				if (_magnificationFactor == value)
					return;

				_magnificationFactor = value;
				RenderImage();
			}
		}

		public PresentationImage SourceImage
		{
			get
			{
				return _sourceImage;
			}
			set
			{
				if (_sourceImage == value)
					return;

				if (value == null)
					throw new ArgumentException("The image cannot be null", "value");

				if (!(value is ISpatialTransformProvider))
					throw new ArgumentException("The image must implement ISpatialTransformProvider", "value");

				if (!(((ISpatialTransformProvider)value).SpatialTransform is ImageSpatialTransform))
					throw new ArgumentException("The image must provide an IImageSpatialTransform", "value");

				DisposeSurface();
				DisposeImage();

				_sourceImage = value;
				_magnificationImage = (PresentationImage)_sourceImage.Clone();
				_renderingSurface = _magnificationImage.ImageRenderer.GetRenderingSurface(Handle, Width, Height);

				HideOverlays();
				
				RenderImage();
			}
		}

		#region Unused Code

		/*
		//text doesn't end up looking very good due to interpolation effects.
		private void AddMagnificationIndicator()
		{
			//Ideally, I would just replace the IAnnotationLayoutProvider with a new one that
			//showed only the mag factor ... but it would actually require framework changes,
			//so I'll do this for now.
			string magFactor = String.Format("{0:F2}x", _magnificationFactor);

			SizeF size;
			Bitmap bitmap = new Bitmap(Width, Height);
			using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
			{
				using (Font font = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point))
				{
					size = graphics.MeasureString(magFactor, font);
				}
			}

			bitmap.Dispose();

			int width = (int) (size.Width + 1) + 4;
			int height = (int)(size.Height + 1) + 4;
			int stride = 4*width;

			byte[] buffer = new byte[stride * height];
			GCHandle bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			try
			{
				bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, bufferHandle.AddrOfPinnedObject());
				using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
				{
					graphics.Clear(Color.FromArgb(0, Color.Black));

					using (Font font = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point))
					{
						//drop-shadow
						using (Brush brush = new SolidBrush(Color.Black))
						{
							graphics.DrawString(magFactor, font, brush, 1, 1);
						}
						
						using (Brush brush = new SolidBrush(Color.WhiteSmoke))
						{
							graphics.DrawString(magFactor, font, brush, 0, 0);
						}
					}
				}
			}
			finally
			{
				bufferHandle.Free();
			}

			ColorImageGraphic graphic = new ColorImageGraphic(bitmap.Height, bitmap.Width, buffer);
			_magnificationImage.SceneGraph.Graphics.Add(graphic);
			graphic.SpatialTransform.TranslationX = Width - graphic.Columns - 5;
			graphic.SpatialTransform.TranslationY = 5;
		}
*/
		#endregion

		public void UpdateMousePosition(Point positionTile)
		{

			Size offsetFromStartTile = new Size(positionTile.X - _startPointTile.X, positionTile.Y - _startPointTile.Y);
			Point pointDesktop = _startPointDesktop;
			pointDesktop.Offset(offsetFromStartTile.Width, offsetFromStartTile.Height);
			Centre = pointDesktop;
		}

		private Point Centre
		{
			get
			{
				Point location = Location;
				location.Offset(Width / 2, Height / 2);
				return location;
			}	
			set
			{
				value.Offset(-Width / 2, -Height / 2);
				if (value != Location)
					base.Location = value;
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//base.OnPaintBackground(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			RefreshImage(e.Graphics);
			//base.OnPaint(e);
		}

		protected override void OnVisibleChanged(System.EventArgs e)
		{
			base.OnVisibleChanged(e);
			RenderImage();
		}

		protected override void OnLocationChanged(System.EventArgs e)
		{
			base.OnLocationChanged(e);

			RenderImage();

			if (base.Visible && base.Owner != null)
				base.Owner.Update(); //update owner's invalidated region(s)
		}

		private void DisposeImage()
		{
			if (_magnificationImage != null)
			{
				_magnificationImage.Dispose();
				_magnificationImage = null;
			}
		}

		private void DisposeSurface()
		{
			if (_renderingSurface != null)
			{
				_renderingSurface.Dispose();
				_renderingSurface = null;
			}
		}

		private void HideOverlays()
		{
			if (_magnificationImage is IAnnotationLayoutProvider)
			{
				string magFactor = String.Format("{0:F1}x", _magnificationFactor);
				AnnotationLayout layout = new AnnotationLayout();
				BasicTextAnnotationItem item = new BasicTextAnnotationItem("mag", "mag", "mag", magFactor);
				AnnotationBox box = new AnnotationBox(new RectangleF(0.8F, 0F, .2F, .05F), item);
				box.Justification = AnnotationBox.JustificationBehaviour.Right;
				box.VerticalAlignment = AnnotationBox.VerticalAlignmentBehaviour.Top;
				layout.AnnotationBoxes.Add(box);
				((BasicPresentationImage)_magnificationImage).AnnotationLayout = layout;
			}

			if (_magnificationImage is IOverlayGraphicsProvider)
			{
				GraphicCollection graphics = ((IOverlayGraphicsProvider) _magnificationImage).OverlayGraphics;
				foreach (IGraphic graphic in graphics)
					graphic.Visible = false;
			}

			if (_magnificationImage is IApplicationGraphicsProvider)
			{
				GraphicCollection graphics = ((IApplicationGraphicsProvider)_magnificationImage).ApplicationGraphics;
				foreach (IGraphic graphic in graphics)
					graphic.Visible = false;
			}

			//we want the Dicom graphics to be visible (e.g. shutter and embedded overlays)

			//if (_magnificationImage is IDicomPresentationImage)
			//{
			//    GraphicCollection graphics = ((IDicomPresentationImage)_magnificationImage).DicomGraphics;
			//    foreach (IGraphic graphic in graphics)
			//        graphic.Visible = false;
			//}
		}

		private void RefreshImage(System.Drawing.Graphics graphics)
		{
			if (!Visible)
				return;

			_renderingSurface.WindowID = Handle;
			_renderingSurface.ContextID = graphics.GetHdc();
			_renderingSurface.ClientRectangle = ClientRectangle;
			_renderingSurface.ClipRectangle = ClientRectangle;

			try
			{
				WinFormsScreenProxy screen = new WinFormsScreenProxy(Screen.FromControl(this));
				DrawArgs args = new DrawArgs(_renderingSurface, screen, ClearCanvas.ImageViewer.Rendering.DrawMode.Refresh);
				_magnificationImage.Draw(args);
			}
			finally
			{
				graphics.ReleaseHdc(_renderingSurface.ContextID);
			}
		}

		private void RenderImage()
		{
			if (!Visible)
				return;

			using (System.Drawing.Graphics graphics = base.CreateGraphics())
			{
				_renderingSurface.WindowID = Handle;
				_renderingSurface.ContextID = graphics.GetHdc();
				_renderingSurface.ClientRectangle = ClientRectangle;
				_renderingSurface.ClipRectangle = ClientRectangle;

				ImageSpatialTransform sourceTransform = (ImageSpatialTransform)((ISpatialTransformProvider)_sourceImage).SpatialTransform;
				ImageSpatialTransform transform = (ImageSpatialTransform)((ISpatialTransformProvider)_magnificationImage).SpatialTransform;

				PointF centerTile = new PointF(_sourceImage.ClientRectangle.Width /2F, _sourceImage.ClientRectangle.Height / 2F);
				SizeF deltaDesktop = new SizeF(Centre.X - _startPointDesktop.X, Centre.Y - _startPointDesktop.Y);
				SizeF startDeltaTile = new SizeF(_startPointTile.X - centerTile.X, _startPointTile.Y - centerTile.Y);

				SizeF renderingOffsetDestination = deltaDesktop + startDeltaTile;
				SizeF renderingOffsetSource = sourceTransform.ConvertToSource(renderingOffsetDestination);

				float scale = sourceTransform.Scale * _magnificationFactor;
				float translationX = sourceTransform.TranslationX - renderingOffsetSource.Width;
				float translationY = sourceTransform.TranslationY - renderingOffsetSource.Height;

				try
				{
					transform.ScaleToFit = false;
					transform.Scale = scale;
					transform.TranslationX = translationX;
					transform.TranslationY = translationY;

					WinFormsScreenProxy screen = new WinFormsScreenProxy(Screen.FromControl(this));
					DrawArgs args = new DrawArgs(_renderingSurface, screen, ClearCanvas.ImageViewer.Rendering.DrawMode.Render);
					_magnificationImage.Draw(args);
				}
				finally
				{
					graphics.ReleaseHdc(_renderingSurface.ContextID);
				}
			}

			Refresh();
		}
	}
}