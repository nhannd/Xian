using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;

namespace ClearCanvas.ImageViewer
{
	public class StandardPresentationImage :
		PresentationImage, 
		IImageGraphicProvider,
		IImageSopProvider, 
		ISpatialTransformProvider,
		IVOILUTLinearProvider,
		IOverlayGraphicsProvider,
		IAnnotationLayoutProvider
	{
		#region Private fields

		private ImageSop _imageSop;
		private CompositeGraphic _compositeImageGraphic;
		private StandardImageGraphic _imageGraphic;
		private CompositeGraphic _overlayGraphics;
		private IAnnotationLayoutProvider _annotationLayoutProvider;

		#endregion

		public StandardPresentationImage(ImageSop imageSop)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");

			_imageSop = imageSop;
			_annotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);

			InitializeSceneGraph();
			InstallDefaultLUTs(_imageGraphic);
		}

		#region Public properties

		public ImageGraphic Image
		{
			get { return _imageGraphic; }
		}

		#region IImageSopProvider members

		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		#endregion

		#region IVOILUTLinearProvider members

		public IVOILUTLinear VoiLut
		{
			get { return _imageGraphic.GrayscaleLUTPipeline.VoiLUT as IVOILUTLinear; }
		}

		#endregion

		#region ISpatialTransformProvider members

		public ISpatialTransform SpatialTransform
		{
			//get { return this.SceneGraph.SpatialTransform; }
			get { return _compositeImageGraphic.SpatialTransform as ISpatialTransform; }
			//get { return _overlayGraphics.SpatialTransform as ISpatialTransform; }
		}

		#endregion

		#region IOverlayGraphicsProvider

		public GraphicCollection OverlayGraphics
		{
			get { return _overlayGraphics.Graphics; }
		}

		#endregion

		#region IAnnotationLayoutProvider Members

		public virtual IAnnotationLayout AnnotationLayout
		{
			get
			{
				if (_annotationLayoutProvider == null)
					return null;

				return _annotationLayoutProvider.AnnotationLayout;
			}
		}

		#endregion

		public IAnnotationLayoutProvider AnnotationLayoutProvider
		{
			get { return _annotationLayoutProvider; }
			protected set { _annotationLayoutProvider = value; }
		}

		public override IRenderer ImageRenderer
		{
			get
			{
				if (_imageRenderer == null)
					_imageRenderer = new StandardPresentationImageRenderer();

				return _imageRenderer;
			}
		}

		#endregion

		#region Disposal

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}

		#endregion

		#region Public methods

		public override IPresentationImage Clone()
		{
			return new StandardPresentationImage(_imageSop);
		}

		public override string ToString()
		{
			return _imageSop.InstanceNumber.ToString();
		}

		#endregion

		private void InitializeSceneGraph()
		{
			_compositeImageGraphic = new CompositeImageGraphic(
				_imageSop.Rows,
				_imageSop.Columns,
				_imageSop.PixelSpacing.Column,
				_imageSop.PixelSpacing.Row);

			_imageGraphic = new StandardImageGraphic(_imageSop);
			_overlayGraphics = new CompositeGraphic();

			_compositeImageGraphic.Graphics.Add(_imageGraphic);
			_compositeImageGraphic.Graphics.Add(_overlayGraphics);
			this.SceneGraph.Graphics.Add(_compositeImageGraphic);

			//StandardImageGraphic image2 = new StandardImageGraphic(_imageSop);
			//image2.SpatialTransform.Scale = 0.25f;
			////image2.SpatialTransform.TranslationX = _imageSop.Columns * 0.75f;
			////image2.SpatialTransform.TranslationY = _imageSop.Rows * 0.75f;

			//InstallDefaultLUTs(image2);
			//this.SceneGraph.Graphics.Add(image2);

			//CompositeGraphic test = new CompositeGraphic();
			//test.SpatialTransform.Rotation = 10;

			//RectanglePrimitive test2 = new RectanglePrimitive();
			//test2.TopLeft = new PointF(100, 100);
			//test2.BottomRight = new PointF(300, 400);

			//test.Graphics.Add(test2);
			//this.SceneGraph.Graphics.Add(test);
		}

		private void InstallDefaultLUTs(StandardImageGraphic imageGraphic)
		{
			if (imageGraphic.IsColor)
				return;

			InstallModalityLUT(imageGraphic);
			InstallVOILUTLinear(imageGraphic);
		}

		private void InstallModalityLUT(StandardImageGraphic imageGraphic)
		{
			if (imageGraphic.GrayscaleLUTPipeline.ModalityLUT == null)
			{
				ModalityLUTLinear modalityLUT =
					new ModalityLUTLinear(
					_imageSop.BitsStored,
					_imageSop.PixelRepresentation,
					_imageSop.RescaleSlope,
					_imageSop.RescaleIntercept);

				imageGraphic.GrayscaleLUTPipeline.ModalityLUT = modalityLUT;
			}
		}

		private void InstallVOILUTLinear(StandardImageGraphic imageGraphic)
		{
			int pixelRepresentation = imageGraphic.ImageSop.PixelRepresentation;
			int bitsStored = imageGraphic.ImageSop.BitsStored;

			ImageValidator.ValidateBitsStored(bitsStored);
			ImageValidator.ValidatePixelRepresentation(pixelRepresentation);

			double windowWidth = double.NaN;
			double windowCenter = double.NaN;

			Window[] windows = imageGraphic.ImageSop.WindowCenterAndWidth;

			if (windows != null)
			{
				windowWidth = windows[0].Width;
				windowCenter = windows[0].Center;
			}

			//Window Width must be non-zero according to DICOM.
			//Otherwise, we want to do something simple (pick 2^BitsStored).
			if (windowWidth == 0 || double.IsNaN(windowWidth))
			{
				windowWidth = 1 << ((int)bitsStored);
			}

			//If Window Center is invalid, calculate a value based on the Window Width.
			if (double.IsNaN(windowCenter))
			{
				if (pixelRepresentation == 0)
				{
					windowCenter = ((int)windowWidth) >> 1;
				}
				else
				{
					windowCenter = 0;
				}
			}

			InstallVOILUTLinear(imageGraphic, windowWidth, windowCenter);
		}

		private void InstallVOILUTLinear(StandardImageGraphic imageGraphic,
			double windowWidth,
			double windowCenter)
		{
			if (this.Image.IsColor)
				return;

			GrayscaleLUTPipeline pipeline = imageGraphic.GrayscaleLUTPipeline;

			VOILUTLinear voiLUT = null;

			// If the pipeline has a VOILUT, check that it's linear
			if (pipeline.VoiLUT != null)
				voiLUT = pipeline.VoiLUT as VOILUTLinear;

			// If the VOILUT on the image is not linear anymore, or
			// if there's no VOILUT on the pipeline to begin with, install a linear one
			if (voiLUT == null)
			{
				IGrayscaleLUT modalityLUT = pipeline.ModalityLUT;
				voiLUT = new VOILUTLinear(modalityLUT.MinOutputValue, modalityLUT.MaxOutputValue);
				pipeline.VoiLUT = voiLUT;
			}

			voiLUT.WindowWidth = windowWidth;
			voiLUT.WindowCenter = windowCenter;
		}
	}
}
