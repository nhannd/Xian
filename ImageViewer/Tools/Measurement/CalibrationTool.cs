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

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;
using ClearCanvas.ImageViewer.StudyManagement;
using System;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("calibrate", "basicgraphic-menu/MenuCalibrationTool", "Calibrate")]
	[EnabledStateObserver("calibrate", "Enabled", "EnabledChanged")]
	[GroupHint("calibrate", "Tools.Image.Measurement.Calibrate")]
	[Tooltip("calibrate", "TooltipCalibrationTool")]

	[ExtensionOf(typeof(GraphicToolExtensionPoint))]
	public class CalibrationTool :GraphicTool
	{
		public CalibrationTool()
		{
		}

		public override void Initialize()
		{
			RoiGraphic roiGraphic = this.Context.OwnerGraphic as RoiGraphic;

			this.Enabled = false;

			// Only allow calibration on linear measurements
			if (roiGraphic != null)
			{
				if (roiGraphic.Roi is IRoiLengthProvider)
					this.Enabled = true;
			}

			base.Initialize();
		}
		
		public void Calibrate()
		{
			CalibrationComponent component;
			
			double length = GetCurrentLength();

			// Show the current length in cm, if it exists
			if (length > 0)
				component = new CalibrationComponent(length);
			else
				component = new CalibrationComponent();

			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Context.DesktopWindow,
				component, 
				SR.CalibrationDialogTitle);

			if (exitCode == ApplicationComponentExitCode.None)
				return;

			double lengthInMm = component.LengthInCm * 10;

			ApplyCalibration(lengthInMm);
		}

		private double GetCurrentLength()
		{
			RoiGraphic roiGraphic = this.Context.OwnerGraphic as RoiGraphic;
			IImageSopProvider image = roiGraphic.ParentPresentationImage as IImageSopProvider;

			Units units = Units.Centimeters;

			double length = RoiLengthAnalyzer.CalculateLength(
				roiGraphic.Subject as PolyLineInteractiveGraphic,
			    image.Frame.NormalizedPixelSpacing, 
				ref units);

			if (units == Units.Centimeters)
				return length;
			else
				return 0.0;
		}

		private void ApplyCalibration(double lengthInMm)
		{
			RoiGraphic roiGraphic = this.Context.OwnerGraphic as RoiGraphic;
			IImageSopProvider image = roiGraphic.ParentPresentationImage as IImageSopProvider;

			double aspectRatio;
			
			if (image.Frame.PixelAspectRatio.IsNull)
			{
				// When there is no aspect ratio tag, the image is displayed with the aspect ratio
				// of the pixel spacing, so just keep the aspect ratio the same as
				// what's displayed.  Otherwise, after calibration, a 2cm line drawn horizontally
				// would be visibly different from a 2cm line drawn vertically.
				if (!image.Frame.NormalizedPixelSpacing.IsNull)
					aspectRatio = image.Frame.NormalizedPixelSpacing.Row / image.Frame.NormalizedPixelSpacing.Column;
				else 
					aspectRatio = 1;
			}
			else
			{
				aspectRatio = image.Frame.PixelAspectRatio.Row / image.Frame.PixelAspectRatio.Column;
			}

			PolyLineInteractiveGraphic line = roiGraphic.Subject as PolyLineInteractiveGraphic;

			line.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = line.PolyLine[1].X - line.PolyLine[0].X;
			double heightInPixels = line.PolyLine[1].Y - line.PolyLine[0].Y;
			line.ResetCoordinateSystem();

			if (widthInPixels == 0 && heightInPixels == 0)
			{
				this.Context.DesktopWindow.ShowMessageBox(SR.ErrorCannotCalibrateZeroLengthRuler, MessageBoxActions.Ok);
				return;
			}

			double pixelSpacingWidth, pixelSpacingHeight;

			CalculatePixelSpacing(
				lengthInMm, 
				widthInPixels, 
				heightInPixels,
				aspectRatio,
				out pixelSpacingWidth,
				out pixelSpacingHeight);

			image.Frame.NormalizedPixelSpacing.Calibrate(pixelSpacingHeight, pixelSpacingWidth);

			if (roiGraphic.ParentPresentationImage is IOverlayGraphicsProvider)
			{
				UpdateRoiGraphics(roiGraphic.ParentPresentationImage as IOverlayGraphicsProvider);
				roiGraphic.ParentPresentationImage.Draw();
			}
		}

		private static void UpdateRoiGraphics(IOverlayGraphicsProvider overlayProvider)
		{
			foreach (IGraphic graphic in overlayProvider.OverlayGraphics)
			{
				if (graphic is RoiGraphic)
				{
					RoiGraphic roiGraphic = graphic as RoiGraphic;
					roiGraphic.ResumeRoiChangedEvent(true);
				}
			}
		}

		public static void CalculatePixelSpacing(
			double lengthInMm, 
			double widthInPixels,
			double heightInPixels,
			double pixelAspectRatio,
			out double pixelSpacingWidth,
			out double pixelSpacingHeight)
		{
			double l2 = lengthInMm*lengthInMm;
			double dx2 = widthInPixels*widthInPixels;
			double dy2 = heightInPixels*heightInPixels;
			double r2 = pixelAspectRatio*pixelAspectRatio;
			pixelSpacingWidth = Math.Sqrt(l2/(dx2 + r2*dy2));
			pixelSpacingHeight = pixelAspectRatio*pixelSpacingWidth;
		}
	}
}
