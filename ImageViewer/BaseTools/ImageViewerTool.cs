using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.BaseTools
{
	/// <summary>
	/// A base class for image viewer tools.
	/// </summary>
	public abstract class ImageViewerTool : Tool<IImageViewerToolContext>
	{
		private bool _enabled = true;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Gets or sets a value indicating whether the tool is enabled.
		/// </summary>
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

		/// <summary>
		/// Occurs when the <see cref="Enabled"/> property has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}
		
		/// <summary>
		/// Gets the <see cref="IImageViewer"/> associated with this tool.
		/// </summary>
		public IImageViewer ImageViewer
		{
			get { return this.Context.Viewer; }
		}

		/// <summary>
		/// Gets the selected <see cref="IPresentationImage"/>.
		/// </summary>
		/// <value>The selected <see cref="IPresentationImage"/> or <b>null</b>
		/// if no <see cref="IPresentationImage"/> is currently selected.</value>
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

		/// <summary>
		/// Gets the selected <see cref="IImageGraphicProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IImageGraphicProvider"/> or <b>null</b>
		/// if no <see cref="IImageGraphicProvider"/> is currently selected.</value>
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

		/// <summary>
		/// Gets the selected <see cref="IImageSopProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IImageSopProvider"/> or <b>null</b>
		/// if no <see cref="IImageSopProvider"/> is currently selected.</value>
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

		/// <summary>
		/// Gets the selected <see cref="ISpatialTransformProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="ISpatialTransformProvider"/> or <b>null</b>
		/// if no <see cref="ISpatialTransformProvider"/> is currently selected.</value>
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

		/// <summary>
		/// Gets the selected <see cref="IVOILUTLinearProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IVOILUTLinearProvider"/> or <b>null</b>
		/// if no <see cref="IVOILUTLinearProvider"/> is currently selected.</value>
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

		/// <summary>
		/// Gets the selected <see cref="IOverlayGraphicsProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IOverlayGraphicsProvider"/> or <b>null</b>
		/// if no <see cref="IOverlayGraphicsProvider"/> is currently selected.</value>
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

		/// <summary>
		/// Gets the selected <see cref="IAnnotationLayoutProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IAnnotationLayoutProvider"/> or <b>null</b>
		/// if no <see cref="IAnnotationLayoutProvider"/> is currently selected.</value>
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
