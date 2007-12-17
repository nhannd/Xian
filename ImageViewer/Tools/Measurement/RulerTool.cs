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
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuRuler", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuMeasurement/MenuRuler", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarMeasurement/ToolbarRuler", "Select", Flags = ClickActionFlags.CheckAction)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.RulerToolSmall.png", "Icons.RulerToolMedium.png", "Icons.RulerToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Measurement.ROI.Linear")]

	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class RulerTool : MouseImageViewerTool
	{
		private static readonly string[] _disallowedModalities = { "CR", "DX", "MG" };
		private RoiGraphic _createGraphic;

		public RulerTool()
			: base(SR.TooltipRuler)
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

			PolyLineInteractiveGraphic polyLineGraphic = new PolyLineInteractiveGraphic(true, 2);
			_createGraphic = new RoiGraphic(polyLineGraphic, true);

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
					command.Name = "Create Ruler";
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

			interactiveGraphic.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = (interactiveGraphic.PolyLine[1].X - interactiveGraphic.PolyLine[0].X);
			double heightInPixels = (interactiveGraphic.PolyLine[1].Y - interactiveGraphic.PolyLine[0].Y);
			interactiveGraphic.ResetCoordinateSystem();

			PixelSpacing pixelSpacing = image.ImageSop.GetModalityPixelSpacing();

			bool pixelSpacingInvalid = pixelSpacing.Row <= float.Epsilon ||
										pixelSpacing.Column <= float.Epsilon ||
										double.IsNaN(pixelSpacing.Row) ||
										double.IsNaN(pixelSpacing.Column);

			string text;

			if (pixelSpacingInvalid)
			{
				double length = Math.Sqrt(widthInPixels * widthInPixels + heightInPixels * heightInPixels);
				text = String.Format(SR.ToolsMeasurementFormatLengthPixels, length);
			}
			else
			{
				double widthInCm = widthInPixels * pixelSpacing.Column / 10;
				double heightInCm = heightInPixels * pixelSpacing.Row / 10;

				double length = Math.Sqrt(widthInCm * widthInCm + heightInCm * heightInCm);
				text = String.Format(SR.ToolsMeasurementFormatLengthCm, length);
			}

			roiGraphic.Callout.Text = text;
		}
	}
}
