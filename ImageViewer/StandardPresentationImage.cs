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
using ClearCanvas.Desktop;

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
		private ImageGraphic _imageGraphic;
		private CompositeGraphic _overlayGraphics;
		private IAnnotationLayoutProvider _annotationLayoutProvider;

		#endregion

		public StandardPresentationImage(ImageSop imageSop)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");

			_imageSop = imageSop;
			_annotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);

			InitializeSceneGraph();
		}

		#region Public properties

		#region IImageGraphicProvider

		public virtual ImageGraphic ImageGraphic
		{
			get { return _imageGraphic; }
		}

		#endregion

		#region IImageSopProvider members

		public virtual ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		#endregion

		#region ISpatialTransformProvider members

		public virtual ISpatialTransform SpatialTransform
		{
			get { return _compositeImageGraphic.SpatialTransform as ISpatialTransform; }
		}

		#endregion

		#region IVOILUTLinearProvider Members

		public virtual IVOILUTLinear VoiLutLinear
		{
			get 
			{
				GrayscaleImageGraphic grayscaleImageGraphic = _imageGraphic as GrayscaleImageGraphic;

				if (grayscaleImageGraphic == null)
					return null;
				else
					return grayscaleImageGraphic.VoiLUTLinear;
			}
		}

		#endregion

		#region IOverlayGraphicsProvider

		public virtual GraphicCollection OverlayGraphics
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

		protected virtual void InitializeSceneGraph()
		{
			_compositeImageGraphic = new CompositeImageGraphic(
				_imageSop.Rows,
				_imageSop.Columns,
				_imageSop.PixelSpacing.Column,
				_imageSop.PixelSpacing.Row);

			_imageGraphic = CreateImageGraphic();
			_overlayGraphics = new CompositeGraphic();

			_compositeImageGraphic.Graphics.Add(_imageGraphic);
			_compositeImageGraphic.Graphics.Add(_overlayGraphics);
			this.SceneGraph.Graphics.Add(_compositeImageGraphic);
		}

		protected virtual ImageGraphic CreateImageGraphic()
		{
			if (_imageSop.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ||
				_imageSop.PhotometricInterpretation == PhotometricInterpretation.Monochrome2)
				return new StandardGrayscaleImageGraphic(_imageSop);
			else if (_imageSop.PhotometricInterpretation == PhotometricInterpretation.PaletteColor)
				return new StandardPaletteColorImageGraphic(_imageSop);
			else
				return new StandardColorImageGraphic(_imageSop);
		}
	}
}
