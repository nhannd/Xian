#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuShowAngles", "ToggleShowAngles", InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuShowAngles", "ToggleShowAngles")]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarShowAngles", "ToggleShowAngles")]
	[CheckedStateObserver("activate", "ShowAngles", "ShowAnglesChanged")]
	[IconSet("activate", "Icons.ShowAnglesToolSmall.png", "Icons.ShowAnglesToolMedium.png", "Icons.ShowAnglesToolLarge.png")]
	[Tooltip("activate", "TooltipShowAngles")]
    [GroupHint("activate", "Tools.Image.Annotations.Measurement.Angle")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class ShowAnglesTool : ImageViewerTool
	{
		private event EventHandler _showAnglesChanged;
		private bool _showAngles = false;

		public override void Initialize()
		{
			base.Initialize();

			base.ImageViewer.EventBroker.GraphicSelectionChanged += OnGraphicSelectionChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.ImageViewer.EventBroker.GraphicSelectionChanged -= OnGraphicSelectionChanged;
			}
			base.Dispose(disposing);
		}

		public bool ShowAngles
		{
			get { return _showAngles; }
			set
			{
				if (_showAngles != value)
				{
					_showAngles = value;
					this.OnShowAnglesChanged();
				}
			}
		}

		protected virtual void OnShowAnglesChanged()
		{
			this.ImageViewer.PhysicalWorkspace.Draw();
			EventsHelper.Fire(_showAnglesChanged, this, EventArgs.Empty);
		}

		public event EventHandler ShowAnglesChanged
		{
			add { _showAnglesChanged += value; }
			remove { _showAnglesChanged -= value; }
		}

		public void ToggleShowAngles()
		{
			this.ShowAngles = !this.ShowAngles;
		}

		private void OnGraphicSelectionChanged(object sender, GraphicSelectionChangedEventArgs e)
		{
			if (e.SelectedGraphic != null)
			{
				IPresentationImage image = e.SelectedGraphic.ParentPresentationImage;
				IOverlayGraphicsProvider overlayGraphicsProvider = image as IOverlayGraphicsProvider;
				if (overlayGraphicsProvider != null)
				{
					ShowAnglesToolCompositeGraphic compositeGraphic = (ShowAnglesToolCompositeGraphic) CollectionUtils.SelectFirst(overlayGraphicsProvider.OverlayGraphics, g => g is ShowAnglesToolCompositeGraphic);
					if (compositeGraphic == null)
					{
						overlayGraphicsProvider.OverlayGraphics.Add(compositeGraphic = new ShowAnglesToolCompositeGraphic(this));
					}
					compositeGraphic.Select(e.SelectedGraphic);
				}
			}
		}
	}
}