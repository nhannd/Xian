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
	/// <summary>
	/// A <see cref="PresentationImage"/> that encapsulates a DICOM image.
	/// </summary>
	public class StandardPresentationImage :
		PresentationImage, 
		IImageGraphicProvider,
		IImageSopProvider, 
		ISpatialTransformProvider,
		IVoiLutManagerProvider,
		IAutoVoiLutApplicatorProvider,
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

		/// <summary>
		/// Initializes a new instance of <see cref="StandardPresentationImage"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		public StandardPresentationImage(ImageSop imageSop)
		{
			Platform.CheckForNullReference(imageSop, "imageSop");

			_imageSop = imageSop;
			_annotationLayoutProvider = new DicomFilteredAnnotationLayoutProvider(this);

			InitializeSceneGraph();
		}

		#region Public properties

		#region IImageGraphicProvider

		/// <summary>
		/// Gets this presentation image's <see cref="ImageGraphic"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="ImageGraphic"/> is the graphical representation of 
		/// the associated the DICOM image.
		/// <see cref="ImageGraphic"/> is the first <see cref="IGraphic"/>
		/// added to the <see cref="SceneGraph"/> and thus is rendered first.
		/// </remarks>
		public virtual ImageGraphic ImageGraphic
		{
			get { return _imageGraphic; }
		}

		#endregion

		#region IImageSopProvider members

		/// <summary>
		/// Gets this presentation image's associated <see cref="ImageSop"/>.
		/// </summary>
		/// <remarks>
		/// Use <see cref="ImageSop"/> to access DICOM tags.
		/// </remarks>
		public virtual ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		#endregion

		#region ISpatialTransformProvider members

		/// <summary>
		/// Gets this presentation image's spatial transform.
		/// </summary>
		/// <remarks>
		/// The <see cref="ImageGraphic"/> and graphics added to the 
		/// <see cref="OverlayGraphics"/> collection are subject to this
		/// spatial transform.  Thus, the effect is that overlay graphics
		/// appear to be anchored to the underlying image.
		/// </remarks>
		public virtual ISpatialTransform SpatialTransform
		{
			get { return _compositeImageGraphic.SpatialTransform as ISpatialTransform; }
		}

		#endregion

		#region IVoiLutManagerProvider Members

		public IVoiLutManager VoiLutManager
		{
			get
			{
				if (_imageGraphic is IVoiLutManagerProvider)
					return (_imageGraphic as IVoiLutManagerProvider).VoiLutManager;

				return null;
			}
		}

		#endregion

		#region IAutoLutApplicatorProvider Members

		/// <summary>
		/// Gets this presentation image's auto Voi Lut Applicator.
		/// </summary>
		/// <value>An <see cref="IAutoVoiLutApplicator"/> or <b>null</b> if the image
		/// is not grayscale.</value>
		/// <remarks>
		/// Use <see cref="AutoVoiLutApplicator"/> to cycle through pre-defined Luts.
		/// </remarks>
		public IAutoVoiLutApplicator AutoVoiLutApplicator
		{
			get
			{
				if (_imageGraphic is IAutoVoiLutApplicatorProvider)
					return (_imageGraphic as IAutoVoiLutApplicatorProvider).AutoVoiLutApplicator;

				return null;
			}
		}

		#endregion

		#region IVOILUTLinearProvider Members

		/// <summary>
		/// Gets this presentation image's linear VOI LUT.
		/// </summary>
		/// <value>An <see cref="IVOILUTLinear"/> or <b>null</b> if the image
		/// is not grayscale, or if the VOILUT currently installed is not linear.</value>
		/// <remarks>
		/// Use <see cref="VoiLutLinear"/> to manipulate window and level.
		/// </remarks>
		public virtual IVOILUTLinear VoiLutLinear
		{
			get
			{
				if (_imageGraphic is IVOILUTLinearProvider)
					return (_imageGraphic as IVOILUTLinearProvider).VoiLutLinear;

				return null;
			}
		}

		#endregion
		
		#region IOverlayGraphicsProvider

		/// <summary>
		/// Gets this presentation image's collection of overlay graphics.
		/// </summary>
		/// <remarks>
		/// Use <see cref="OverlayGraphics"/> to add graphics that you want to
		/// overlay the image.
		/// </remarks>
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

		/// <summary>
		/// Gets a <see cref="StandardPresentationImageRenderer"/>.
		/// </summary>
		/// <remarks>
		/// In general, <see cref="ImageRenderer"/> should be considered an internal
		/// Framework property and should not be used.
		/// </remarks>
		public override IRenderer ImageRenderer
		{
			get
			{
				if (base.ImageRenderer == null)
					base.ImageRenderer = new StandardPresentationImageRenderer();

				return base.ImageRenderer;
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

		/// <summary>
		/// Creates a clone of the <see cref="StandardPresentationImage"/>.
		/// </summary>
		/// <returns></returns>
		public override IPresentationImage Clone()
		{
			IPresentationImage image = new StandardPresentationImage(_imageSop);
			image.Uid = this.Uid;

			return image;
		}

		public override string ToString()
		{
			return _imageSop.InstanceNumber.ToString();
		}

		#endregion

		/// <summary>
		/// Initializes the <see cref="SceneGraph"/>.
		/// </summary>
		protected virtual void InitializeSceneGraph()
		{
			_compositeImageGraphic = new CompositeImageGraphic(
				_imageSop.Rows,
				_imageSop.Columns,
				_imageSop.PixelSpacing.Column,
				_imageSop.PixelSpacing.Row,
				_imageSop.PixelAspectRatio.Column,
				_imageSop.PixelAspectRatio.Row);

			_imageGraphic = CreateImageGraphic();
			_overlayGraphics = new CompositeGraphic();

			//ColorImageGraphic colorOverlay = new ColorImageGraphic(_imageSop.Rows, _imageSop.Columns, null);

			//_imageGraphic.PixelData.ForEachPixel(
			//    delegate(int i, int x, int y, int pixelIndex)
			//    {
			//        if (_imageGraphic.PixelData.GetPixel(pixelIndex) > 1200)
			//            colorOverlay.PixelData.SetPixel(pixelIndex, Color.FromArgb(150, 250, 0, 200).ToArgb());
			//    });

			_compositeImageGraphic.Graphics.Add(_imageGraphic);
			//_compositeImageGraphic.Graphics.Add(colorOverlay);
			_compositeImageGraphic.Graphics.Add(_overlayGraphics);
			this.SceneGraph.Graphics.Add(_compositeImageGraphic);
		}

		/// <summary>
		/// Creates the <see cref="ImageGraphic"/>.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// The appropriate <see cref="ImageGraphic"/> subclass is created
		/// depending on the image's photometric interpretation.
		/// </para>
		/// <para>
		/// Override <see cref="CreateImageGraphic"/> if you want a different type
		/// of <see cref="ImageGraphic"/>.
		/// </para>
		/// </remarks>
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
