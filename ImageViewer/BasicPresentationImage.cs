using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// The <see cref="BasicPresentationImage"/> uses this ExtensionPoint to load a single <see cref="IRenderer"/>
	/// object that is used for all <see cref="BasicPresentationImage"/>s.  Implementations of <IRenderer> that 
	/// extend this <see cref="ExtensionPoint"/> should be thread-safe to account for the possibility that 
	/// <see cref="BasicPresentationImage"/>s could be rendered on multiple UI threads.
	/// </summary>
	public sealed class BasicPresentationImageRendererExtensionPoint : ExtensionPoint<IRenderer>
	{
		public BasicPresentationImageRendererExtensionPoint()
		{
		}
	}

	/// <summary>
	/// A <see cref="PresentationImage"/> that encapsulates a DICOM image.
	/// </summary>
	public abstract class BasicPresentationImage :
		PresentationImage, 
		IPixelDataProvider,
		IImageGraphicProvider,
		ISpatialTransformProvider,
		IOverlayGraphicsProvider,
		IAnnotationLayoutProvider
	{
		#region Private fields

		private static readonly object _rendererSyncLock = new object();
		private static volatile IRenderer _renderer;

		private CompositeGraphic _compositeImageGraphic;
		private ImageGraphic _imageGraphic;
		private CompositeGraphic _overlayGraphics;
		private IAnnotationLayoutProvider _annotationLayoutProvider;

		#endregion

		protected BasicPresentationImage(
			ImageGraphic imageGraphic) 
			: this(imageGraphic, 1.0, 1.0, 1.0, 1.0)
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage"/>.
		/// </summary>
		protected BasicPresentationImage(
			ImageGraphic imageGraphic,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY)
		{
			Platform.CheckForNullReference(imageGraphic, "imageGraphic");

			InitializeSceneGraph(
				imageGraphic, 
				pixelSpacingX, 
				pixelSpacingY, 
				pixelAspectRatioX, 
				pixelAspectRatioY);
		}

		#region Public properties

		#region IPixelDataProvider Members

		/// <summary>
		/// Gets this presentation image's <see cref="PixelData"/>.
		/// </summary>
		public PixelData PixelData
		{
			get { return _imageGraphic.PixelData; }
		}

		#endregion

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
		public ImageGraphic ImageGraphic
		{
			get { return _imageGraphic; }
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
		public ISpatialTransform SpatialTransform
		{
			get { return _compositeImageGraphic.SpatialTransform; }
		}

		#endregion

		/// <summary>
		/// Gets this presentation image's collection of overlay graphics.
		/// </summary>
		/// <remarks>
		/// Use <see cref="OverlayGraphics"/> to add graphics that you want to
		/// overlay the image.
		/// </remarks>
		public GraphicCollection OverlayGraphics
		{
			get { return _overlayGraphics.Graphics; }
		}

		#endregion

		#region IAnnotationLayoutProvider Members

		public IAnnotationLayout AnnotationLayout
		{
			get
			{
				if (_annotationLayoutProvider == null)
					return null;

				return _annotationLayoutProvider.AnnotationLayout;
			}
		}

		#endregion

		protected IAnnotationLayoutProvider AnnotationLayoutProvider
		{
			get { return _annotationLayoutProvider; }			
			set { _annotationLayoutProvider = value; }
		}

		/// <summary>
		/// Gets a <see cref="IRenderer"/>.
		/// </summary>
		/// <remarks>
		/// In general, <see cref="ImageRenderer"/> should be considered an internal
		/// Framework property and should not be used.
		/// </remarks>
		public override IRenderer ImageRenderer
		{
			get
			{
				if (_renderer == null)
				{
					lock (_rendererSyncLock)
					{
						if (_renderer == null)
						{
							try
							{
								_renderer = new BasicPresentationImageRendererExtensionPoint().CreateExtension() as IRenderer;
							}
							catch (Exception e)
							{
								//this isn't an error, just log it as information.
								Platform.Log(LogLevel.Info, e);
							}

							if (_renderer == null)
								_renderer = new GDIRenderer();
						}
					}
				}

				return _renderer;
			}
		}

		#region Disposal

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}

		#endregion

		private void InitializeSceneGraph(
			ImageGraphic imageGraphic,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY)
		{
			_imageGraphic = imageGraphic;

			_compositeImageGraphic = new CompositeImageGraphic(
				imageGraphic.Rows,
				imageGraphic.Columns,
				pixelSpacingX,
				pixelSpacingY,
				pixelAspectRatioX,
				pixelAspectRatioY);

			_overlayGraphics = new CompositeGraphic();

			_compositeImageGraphic.Graphics.Add(_imageGraphic);
			_compositeImageGraphic.Graphics.Add(_overlayGraphics);
			this.SceneGraph.Graphics.Add(_compositeImageGraphic);
		}
	}
}
