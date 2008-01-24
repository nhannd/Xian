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

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarEllipticalRoi", "Select", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.EllipticalRoiToolSmall.png", "Icons.EllipticalRoiToolMedium.png", "Icons.EllipticalRoiToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Measurement.Roi.Elliptical")]

	[MouseToolButton(XMouseButtons.Left, false)]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class EllipticalRoiTool : MouseImageViewerTool
	{
		private RoiGraphic _createGraphic;

		public EllipticalRoiTool()
			: base(SR.TooltipEllipticalRoi)
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

			//When you create a graphic from within a tool (particularly one that needs capture, like a multi-click graphic),
			//see it through to the end of creation.  It's just cleaner, not to mention that if this tool knows how to create it,
			//it should also know how to (and be responsible for) cancelling it and/or deleting it appropriately.
			EllipseInteractiveGraphic ellipseGraphic = new EllipseInteractiveGraphic(true);
			_createGraphic = new RoiGraphic(ellipseGraphic, true);

			_createGraphic.Roi.ControlPoints.Visible = false;
			_createGraphic.Callout.Text = SR.ToolsMeasurementArea;
			image.OverlayGraphics.Add(_createGraphic);
			_createGraphic.RoiChanged += new EventHandler(OnRoiChanged);

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
					command.Name = "Create Ellipse";
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

		private void OnRoiChanged(object sender, EventArgs e)
		{
			RoiGraphic roiGraphic = sender as RoiGraphic;

			EllipseInteractiveGraphic ellipseGraphic = roiGraphic.Roi as EllipseInteractiveGraphic;
			IImageSopProvider image = roiGraphic.ParentPresentationImage as IImageSopProvider;

			ellipseGraphic.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = (ellipseGraphic.BottomRight.X - ellipseGraphic.TopLeft.X);
			double heightInPixels = (ellipseGraphic.BottomRight.Y - ellipseGraphic.TopLeft.Y);
			ellipseGraphic.ResetCoordinateSystem();

			PixelSpacing pixelSpacing = image.ImageSop.GetModalityPixelSpacing();

			bool pixelSpacingInvalid = pixelSpacing.Row <= float.Epsilon ||
										pixelSpacing.Column <= float.Epsilon ||
										double.IsNaN(pixelSpacing.Row) ||
										double.IsNaN(pixelSpacing.Column);
			string text;

			if (pixelSpacingInvalid)
			{
				double area = Math.Abs(widthInPixels * heightInPixels);
				text = String.Format(SR.ToolsMeasurementFormatAreaPixels, area);
			}
			else
			{
				double widthInCm = widthInPixels * pixelSpacing.Column / 10;
				double heightInCm = heightInPixels * pixelSpacing.Row / 10;

				double area = Math.Abs(widthInCm * heightInCm);
				text = String.Format(SR.ToolsMeasurementFormatAreaSquareCm, area);
			}

			roiGraphic.Callout.Text = text;
		}
	}
}