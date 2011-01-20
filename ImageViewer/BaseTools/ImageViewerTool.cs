#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InputManagement;
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
		/// Protected constructor.
		/// </summary>
		protected ImageViewerTool()
		{
		}

		/// <summary>
		/// Disposes of this object; override this method to do any necessary cleanup.
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized.</param>
		protected override void Dispose(bool disposing)
		{
			this.Context.Viewer.EventBroker.TileSelected -= new EventHandler<TileSelectedEventArgs>(OnTileSelected);
			this.Context.Viewer.EventBroker.PresentationImageSelected -= new EventHandler<PresentationImageSelectedEventArgs>(OnPresentationImageSelected);

			base.Dispose(disposing);
		}

		/// <summary>
		/// Initializes the <see cref="ImageViewerTool"/>.
		/// </summary>
		public override void Initialize()
		{
			this.Context.Viewer.EventBroker.TileSelected += new EventHandler<TileSelectedEventArgs>(OnTileSelected);
			this.Context.Viewer.EventBroker.PresentationImageSelected += new EventHandler<PresentationImageSelectedEventArgs>(OnPresentationImageSelected);

			base.Initialize();
		}

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
		protected IImageViewer ImageViewer
		{
			get { return this.Context.Viewer; }
		}

		/// <summary>
		/// Gets the selected <see cref="IPresentationImage"/>.
		/// </summary>
		/// <value>The selected <see cref="IPresentationImage"/> or <b>null</b>
		/// if no <see cref="IPresentationImage"/> is currently selected.</value>
		protected IPresentationImage SelectedPresentationImage
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
		protected IImageGraphicProvider SelectedImageGraphicProvider
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
		protected IImageSopProvider SelectedImageSopProvider
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
		protected ISpatialTransformProvider SelectedSpatialTransformProvider
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
		/// Gets the selected <see cref="IVoiLutProvider"/>.
		/// </summary>
		/// <value>The selected <see cref="IVoiLutProvider"/> or <b>null</b>
		/// if no <see cref="IVoiLutProvider"/> is currently selected.</value>
		protected IVoiLutProvider SelectedVoiLutProvider
		{
			get
			{
				if (this.SelectedPresentationImage != null)
				{
					return this.SelectedPresentationImage as IVoiLutProvider;
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
		protected IOverlayGraphicsProvider SelectedOverlayGraphicsProvider
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
		protected IAnnotationLayoutProvider SelectedAnnotationLayoutProvider
		{
			get
			{
				if (this.SelectedPresentationImage != null)
					return this.SelectedPresentationImage as IAnnotationLayoutProvider;
				else
					return null;
			}
		}

		/// <summary>
		/// Event Handler for <see cref="EventBroker.TileSelected"/>.
		/// </summary>
		protected virtual void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			if (e.SelectedTile.PresentationImage == null)
				this.Enabled = false;
			else
				this.Enabled = true;
		}

		/// <summary>
		/// Event Handler for <see cref="EventBroker.PresentationImageSelected"/>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			if (e.SelectedPresentationImage == null)
				this.Enabled = false;
			else
				this.Enabled = true;
		}
	}
}
