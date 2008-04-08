#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer
{
	#region BasicPresentationImageRendererFactoryExtensionPoint class

	/// <summary>
	/// The <see cref="BasicPresentationImage"/> creates a single factory object that is then used to create
	/// an <see cref="IRenderer"/> for each <see cref="BasicPresentationImage"/>.  
	/// </summary>
	/// <remarks>
	/// The returned <see cref="IRenderer"/> need not be thread-safe as 
	/// the <see cref="BasicPresentationImage"/> itself is not thread-safe.
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

	#endregion

	/// <summary>
	/// A <see cref="PresentationImage"/> that encapsulates basic
	/// 2D image functionality.
	/// </summary>
	[Cloneable]
	public abstract class BasicPresentationImage :
		PresentationImage, 
		IImageGraphicProvider,
		ISpatialTransformProvider,
		IOverlayGraphicsProvider,
		IAnnotationLayoutProvider
	{
		#region Private fields

		private static readonly object _rendererFactorySyncLock = new object();
		private static volatile IRendererFactory _rendererFactory;
		private static volatile RendererFactoryInitializationException _rendererFactoryInitializationException;

		[CloneIgnore]
		private CompositeGraphic _compositeImageGraphic;
		[CloneIgnore]
		private ImageGraphic _imageGraphic;
		[CloneIgnore]
		private CompositeGraphic _overlayGraphics;
		private IAnnotationLayoutProvider _annotationLayoutProvider;

		#endregion

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected BasicPresentationImage(BasicPresentationImage source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="BasicPresentationImage"/>.
		/// </summary>
		protected BasicPresentationImage(ImageGraphic imageGraphic) 
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

		#region IImageGraphicProvider

		/// <summary>
		/// Gets this presentation image's <see cref="ImageGraphic"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="ImageGraphic"/> is the first <see cref="IGraphic"/>
		/// added to the <see cref="PresentationImage.SceneGraph"/> and thus is rendered first.
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
				if (AnnotationLayoutProvider == null)
					return ClearCanvas.ImageViewer.Annotations.AnnotationLayout.Empty;

				return AnnotationLayoutProvider.AnnotationLayout ?? ClearCanvas.ImageViewer.Annotations.AnnotationLayout.Empty;
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
		/// Gets an <see cref="IRenderer"/>.
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
								_rendererFactory.Initialize();
							}
							catch (RendererFactoryInitializationException e)
							{
								_rendererFactory = null;
								_rendererFactoryInitializationException = e;
								Platform.Log(LogLevel.Warn, e);
							}
							catch(NotSupportedException)
							{
								Platform.Log(LogLevel.Info, SR.MessageNoRendererPluginsExist);
							}
							catch (Exception e)
							{
								Platform.Log(LogLevel.Warn, e);
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

		/// <summary>
		/// Raises the <see cref="EventBroker.ImageDrawing"/> event and
		/// renders the <see cref="PresentationImage"/>.
		/// </summary>
		/// <param name="drawArgs"></param>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Draw(DrawArgs drawArgs)
		{
			base.Draw(drawArgs);

			CheckRendererInitializationError();
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

		private static void CheckRendererInitializationError()
		{
			if (_rendererFactoryInitializationException != null)
			{
				Exception e = _rendererFactoryInitializationException;
				_rendererFactoryInitializationException = null;
				throw e;
			}
		}

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
			_overlayGraphics.Name = "Overlay";

			_compositeImageGraphic.Graphics.Add(_imageGraphic);
			_compositeImageGraphic.Graphics.Add(_overlayGraphics);
			this.SceneGraph.Graphics.Add(_compositeImageGraphic);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_compositeImageGraphic = CollectionUtils.SelectFirst(SceneGraph.Graphics,
				delegate(IGraphic test) { return test is CompositeImageGraphic; }) as CompositeImageGraphic;

			Platform.CheckForNullReference(_compositeImageGraphic, "_compositeImageGraphic");

			_imageGraphic = CollectionUtils.SelectFirst(_compositeImageGraphic.Graphics,
				delegate(IGraphic test) { return test is ImageGraphic; }) as ImageGraphic;

			_overlayGraphics = CollectionUtils.SelectFirst(_compositeImageGraphic.Graphics,
				delegate(IGraphic test) { return test.Name == "Overlay"; }) as CompositeGraphic;

			Platform.CheckForNullReference(_imageGraphic, "_imageGraphic");
			Platform.CheckForNullReference(_overlayGraphics, "_overlayGraphics");
		}
	}
}
