#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	[IconSet("activate", IconScheme.Colour, "Icons.ShowAnglesToolSmall.png", "Icons.ShowAnglesToolMedium.png", "Icons.ShowAnglesToolLarge.png")]
	[Tooltip("activate", "TooltipShowAngles")]
	[GroupHint("activate", "Tools.Image.Measurement.Angle")]
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