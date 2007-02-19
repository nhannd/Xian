using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.BaseTools
{
	public abstract class ImageViewerTool : Tool<IImageViewerToolContext>
	{
		private bool _enabled = true;
		private event EventHandler _enabledChanged;

		public ImageViewerTool()
		{

		}

		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}
		
		/// <summary>
		/// Provides access to the <see cref="IImageViewer"/> associated with this tool.
		/// </summary>
		public IImageViewer ImageViewer
		{
			get { return this.Context.Viewer; }
		}

		public IPresentationImage SelectedPresentationImage
		{
			get
			{
				if (this.ImageViewer != null)
					return this.ImageViewer.SelectedPresentationImage;
				else
					return null;
			}
		}

		public IImageGraphicProvider SelectedImageGraphicProvider
		{
			get
			{
				if (this.SelectedPresentationImage != null)
					return this.SelectedPresentationImage as IImageGraphicProvider;
				else
					return null;
			}
		}

		public IImageSopProvider SelectedImageSopProvider
		{
			get
			{
				if (this.SelectedPresentationImage != null)
					return this.SelectedPresentationImage as IImageSopProvider;
				else
					return null;
			}
		}

		public ISpatialTransformProvider SelectedSpatialTransformProvider
		{
			get
			{
				if (this.SelectedPresentationImage != null)
					return this.SelectedPresentationImage as ISpatialTransformProvider;
				else
					return null;
			}
		}

		public IVOILUTLinearProvider SelectedVOILUTLinearProvider
		{
			get
			{
				if (this.SelectedPresentationImage != null)
				{
					return this.SelectedPresentationImage as IVOILUTLinearProvider;
				}
				else
					return null;
			}
		}

		public IOverlayGraphicsProvider SelectedOverlayGraphicsProvider
		{
			get
			{
				if (this.SelectedPresentationImage != null)
					return this.SelectedPresentationImage as IOverlayGraphicsProvider;
				else
					return null;
			}
		}

		public IAnnotationLayoutProvider SelectedAnnotationLayoutProvider
		{
			get
			{
				if (this.SelectedPresentationImage != null)
					return this.SelectedPresentationImage as IAnnotationLayoutProvider;
				else
					return null;
			}
		}
	}
}
