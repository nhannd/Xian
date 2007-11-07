#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuRectangularROI", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarRectangularROI", "Select", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.RectangularROIToolSmall.png", "Icons.RectangularROIToolMedium.png", "Icons.RectangularROIToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Measurement.ROI.Rectangular")]

	[MouseToolButton(XMouseButtons.Left, false)]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RectangularROITool : MouseImageViewerTool
	{
		private static readonly string[] _disallowedModalities = { "CR", "DX", "MG" };
		private RoiGraphic _createGraphic;

		public RectangularROITool()
			: base(SR.TooltipRectangularROI)
		{
			this.Behaviour = MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		public override string Tooltip
		{
			get { return base.Tooltip; }
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
			RectangleInteractiveGraphic rectangleGraphic = new RectangleInteractiveGraphic(true);
			_createGraphic = new RoiGraphic(rectangleGraphic, true);

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
					return true;
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

		private bool PixelSpacingNotAllowed(ImageSop imageSop)
		{
			foreach (string modality in _disallowedModalities)
			{
				if (String.Compare(modality, imageSop.Modality, true) == 0)
					return true;
			}

			return false;
		}

		private void OnRoiChanged(object sender, EventArgs e)
		{
			RoiGraphic roiGraphic = sender as RoiGraphic;

			RectangleInteractiveGraphic rectangleGraphic = roiGraphic.Roi as RectangleInteractiveGraphic;
			IImageSopProvider image = roiGraphic.ParentPresentationImage as IImageSopProvider;

			rectangleGraphic.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = (rectangleGraphic.BottomRight.X - rectangleGraphic.TopLeft.X);
			double heightInPixels = (rectangleGraphic.BottomRight.Y - rectangleGraphic.TopLeft.Y);
			rectangleGraphic.ResetCoordinateSystem();

			double pixelSpacingX;
			double pixelSpacingY;

			ImageSopHelper.GetModalityPixelSpacing(image.ImageSop, out pixelSpacingX, out pixelSpacingY);

			bool pixelSpacingInvalid =  pixelSpacingX <= float.Epsilon ||
										pixelSpacingY <= float.Epsilon ||
										double.IsNaN(pixelSpacingX) ||
										double.IsNaN(pixelSpacingY);

			string text;

			if (pixelSpacingInvalid)
			{
				double area = Math.Abs(widthInPixels * heightInPixels);
				text = String.Format(SR.ToolsMeasurementFormatAreaPixels, area);
			}
			else
			{
				double widthInCm = widthInPixels * pixelSpacingX / 10;
				double heightInCm = heightInPixels * pixelSpacingY / 10;
				double area = Math.Abs(widthInCm * heightInCm);
				text = String.Format(SR.ToolsMeasurementFormatAreaSquareCm, area);
			}

			roiGraphic.Callout.Text = text;
		}
	}
}