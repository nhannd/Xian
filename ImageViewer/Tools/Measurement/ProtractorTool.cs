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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuProtractor", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuProtractor", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarProtractor", "Select", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ProtractorToolSmall.png", "Icons.ProtractorToolMedium.png", "Icons.ProtractorToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Measurement.Angle")]

	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class ProtractorTool : MouseImageViewerTool
	{
		private RoiGraphic _createGraphic;

		public ProtractorTool()
			: base(SR.TooltipProtractor)
		{
			this.Behaviour = MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			IOverlayGraphicsProvider image = mouseInformation.Tile.PresentationImage as IOverlayGraphicsProvider;

			if (image == null)
				return false;

			if (_createGraphic != null)
				return _createGraphic.Start(mouseInformation);

			PolyLineInteractiveGraphic polyLineGraphic = new PolyLineInteractiveGraphic(true, 3);
			_createGraphic = new RoiGraphic(polyLineGraphic, true);

			image.OverlayGraphics.Add(_createGraphic);
			_createGraphic.RoiChanged += OnRoiChanged;

			// Hide the callout when first being drawn, since there's
			// no angle to display yet.
			_createGraphic.Callout.Visible = false;

			if (_createGraphic.Start(mouseInformation))
				return true;

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_createGraphic != null)
				return _createGraphic.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_createGraphic != null)
			{
				if (_createGraphic.Stop(mouseInformation))
				{
					IOverlayGraphicsProvider image = (IOverlayGraphicsProvider)mouseInformation.Tile.PresentationImage;

					InsertRemoveGraphicUndoableCommand command = InsertRemoveGraphicUndoableCommand.GetRemoveCommand(image.OverlayGraphics, _createGraphic);
					command.Name = "Create Protractor Tool";
					_createGraphic.ImageViewer.CommandHistory.AddCommand(command);
					return true;
				}
			}

			_createGraphic = null;
			return false;
		}

		public override void Cancel()
		{
			if (_createGraphic != null)
				_createGraphic.Cancel();

			IOverlayGraphicsProvider image = _createGraphic.ParentPresentationImage as IOverlayGraphicsProvider;
			image.OverlayGraphics.Remove(_createGraphic);

			_createGraphic.ParentPresentationImage.Draw();
			_createGraphic = null;
		}

		public override CursorToken GetCursorToken(Point point)
		{
			if (_createGraphic != null)
				return _createGraphic.GetCursorToken(point);

			return null;
		}

		// This is temporary code.  Right now, the api is difficult to use.  
		// Ideally, we should have domain objects that make this easier.  
		private void OnRoiChanged(object sender, EventArgs e)
		{
			RoiGraphic roiGraphic = sender as RoiGraphic;
			
			PolyLineInteractiveGraphic interactiveGraphic = roiGraphic.Roi as PolyLineInteractiveGraphic;
			IImageSopProvider image = roiGraphic.ParentPresentationImage as IImageSopProvider;

			// Don't show the callout until the second ray drawn
			if (interactiveGraphic.PolyLine.Count < 3)
				return;

			roiGraphic.Callout.Visible = true;
			interactiveGraphic.CoordinateSystem = CoordinateSystem.Destination;

			double angle = Vector.SubtendedAngle(
				interactiveGraphic.PolyLine[0],
				interactiveGraphic.PolyLine[1],
				interactiveGraphic.PolyLine[2]);

			roiGraphic.Callout.Text = String.Format("{0:F2}°", angle);

			interactiveGraphic.ResetCoordinateSystem();
		}
	}
}
