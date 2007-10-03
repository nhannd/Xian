using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// The <see cref="BasicPresentationImage"/> creates a single factory object that is then used to create
	/// an <see cref="IRenderer"/> for each <see cref="BasicPresentationImage"/>.  
	/// </summary>
	/// <remarks>
	/// The returned <see cref="IRenderer"/>
	/// need not be thread-safe as the <see cref="BasicPresentationImage"/> itself is not thread-safe.
	/// </remarks>
	public sealed class BasicPresentationImageRendererFactoryExtensionPoint : ExtensionPoint<IRendererFactory>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public BasicPresentationImageRendererFactoryExtensionPoint()
		{
		}
	}

	/// <summary>
	/// A <see cref="PresentationImage"/> that encapsulates basic
	/// 2D image functionality.
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

		private static readonly object _rendererFactorySyncLock = new object();
		private static volatile IRendererFactory _rendererFactory;

		private CompositeGraphic _compositeImageGraphic;
		private ImageGraphic _imageGraphic;
		private CompositeGraphic _overlayGraphics;
		private IAnnotationLayoutProvider _annotationLayoutProvider;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage"/>.
		/// </summary>
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

		/// <summary>
		/// Gets the <see cref="IAnnotationLayout"/> associated with this presentation image.
		/// </summary>
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

		/// <summary>
		/// Gets the associated <see cref="IAnnotationLayoutProvider"/> for this image.
		/// </summary>
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
				if (_rendererFactory == null)
				{
					lock (_rendererFactorySyncLock)
					{
						if (_rendererFactory == null)
						{
							try
							{
								_rendererFactory = (IRendererFactory)new BasicPresentationImageRendererFactoryExtensionPoint().CreateExtension();
							}
							catch (Exception e)
							{
								//this isn't an error, just log it as information.
								Platform.Log(LogLevel.Info, e);
							}

							if (_rendererFactory == null)
								_rendererFactory = new GdiRendererFactory();
						}
					}
				}

				if (base.ImageRenderer == null)
					base.ImageRenderer = _rendererFactory.GetRenderer();

				return base.ImageRenderer;
			}
		}

		#region Disposal

		/// <summary>
		/// Dispose method.  Inheritors should override this method to do any additional cleanup.
		/// </summary>
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
