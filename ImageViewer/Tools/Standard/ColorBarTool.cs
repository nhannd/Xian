﻿#region License

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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideColorBar", "ShowHide")]
	[EnabledStateObserver("showHide", "Enabled", "EnabledChanged")]
	[Tooltip("showHide", "TooltipShowHideColorBar")]
	[GroupHint("showHide", "Tools.Image.Overlays.ColourBar.ShowHide")]
	[IconSet("showHide", IconScheme.Colour, "Icons.ColorBarToolSmall.png", "Icons.ColorBarToolMedium.png", "Icons.ColorBarToolLarge.png")]
	//
	[MenuAction("toggle", "overlays-dropdown/ToolbarColorBar", "ShowHide")]
	[EnabledStateObserver("toggle", "Enabled", "EnabledChanged")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[Tooltip("toggle", "TooltipColorBar")]
	[GroupHint("toggle", "Tools.Image.Overlays.ColourBar.ShowHide")]
	[IconSet("toggle", IconScheme.Colour, "Icons.ColorBarToolSmall.png", "Icons.ColorBarToolMedium.png", "Icons.ColorBarToolLarge.png")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ColorBarTool : OverlayToolBase
	{
		private bool _enabled;

		public event EventHandler EnabledChanged;

		public ColorBarTool()
		{
			this.Checked = false;
		}

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		protected override void UpdateVisibility(IPresentationImage image, bool visible)
		{
			ColorBarCompositeGraphic graphic = GetCompositeScaleGraphic(image, visible);
			if (graphic != null)
				graphic.Visible = visible;
		}

		private static ColorBarCompositeGraphic GetCompositeScaleGraphic(IPresentationImage image, bool createIfNull)
		{
			if (image is IColorMapProvider && image is IApplicationGraphicsProvider)
			{
				GraphicCollection applicationGraphics = ((IApplicationGraphicsProvider) image).ApplicationGraphics;
				ColorBarCompositeGraphic graphic = (ColorBarCompositeGraphic) CollectionUtils.SelectFirst(applicationGraphics, g => g is ColorBarCompositeGraphic);

				if (graphic == null && createIfNull)
					applicationGraphics.Add(graphic = new ColorBarCompositeGraphic());

				return graphic;
			}

			return null;
		}

		public override void Initialize()
		{
			base.Initialize();

			base.Context.Viewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.Context.Viewer.EventBroker.PresentationImageSelected -= OnPresentationImageSelected;
			}

			base.Dispose(disposing);
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			this.Enabled = e.SelectedPresentationImage is IColorMapProvider;
		}

		[Cloneable]
		private class ColorBarCompositeGraphic : CompositeGraphic
		{
			[CloneIgnore]
			private ColorBarGraphic _colorBarGraphic;

			public ColorBarCompositeGraphic()
			{
				base.Graphics.Add(_colorBarGraphic = new ColorBarGraphic());
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected ColorBarCompositeGraphic(ColorBarCompositeGraphic source, ICloningContext context)
			{
				context.CloneFields(source, this);
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				_colorBarGraphic = (ColorBarGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ColorBarGraphic);
			}

			public override void OnDrawing()
			{
				// ensure the color bar uses the same color map as the underlying image
				if (base.ParentPresentationImage is IColorMapProvider)
					_colorBarGraphic.ColorMapManager.SetMemento(((IColorMapProvider) base.ParentPresentationImage).ColorMapManager.CreateMemento());
				if (base.ParentPresentationImage != null)
				{
					_colorBarGraphic.CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						_colorBarGraphic.Length = (int) (base.ParentPresentationImage.ClientRectangle.Height*0.3f);
						_colorBarGraphic.Location = new PointF(25, (base.ParentPresentationImage.ClientRectangle.Height - _colorBarGraphic.Length)/2f);
					}
					finally
					{
						_colorBarGraphic.ResetCoordinateSystem();
					}
				}

				base.OnDrawing();
			}
		}
	}
}