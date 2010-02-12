#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;
using ClearCanvas.ImageViewer.StudyManagement;
using System;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[MenuAction("calibrate", "basicgraphic-menu/MenuCalibrationTool", "Calibrate")]
	[VisibleStateObserver("calibrate", "Visible", "VisibleChanged")]
	[GroupHint("calibrate", "Tools.Image.Measurement.Calibrate")]
	[Tooltip("calibrate", "TooltipCalibrationTool")]

	[ExtensionOf(typeof(GraphicToolExtensionPoint))]
	public class CalibrationTool : GraphicTool
	{
		public CalibrationTool()
		{
		}

		private RoiGraphic RoiGraphic
		{
			get { return Context.Graphic as RoiGraphic; }	
		}

		private IPointsGraphic LineGraphic
		{
			get
			{
				RoiGraphic graphic = RoiGraphic;
				if (graphic != null)
				{
					IPointsGraphic lineGraphic = graphic.Subject as IPointsGraphic;
					if (lineGraphic != null && lineGraphic.Points.Count == 2)
						return lineGraphic;
				}

				return null;
			}	
		}

		private Frame Frame
		{
			get
			{
				RoiGraphic graphic = RoiGraphic;
				if (graphic != null)
				{
					if (graphic.ParentPresentationImage is IImageSopProvider)
						return ((IImageSopProvider)graphic.ParentPresentationImage).Frame;
				}

				return null;
			}	
		}

		private bool IsValidGraphic()
		{
			return LineGraphic != null && Frame != null;
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Visible = IsValidGraphic();
		}
		
		public void Calibrate()
		{
			if (!Visible)
				return;

			double length = GetCurrentLength();

			// Show the current length in cm, if it exists
			CalibrationComponent component = length > 0 ? new CalibrationComponent(length) : new CalibrationComponent();

			if (ApplicationComponentExitCode.Accepted != ApplicationComponent.LaunchAsDialog(
				Context.DesktopWindow, component, SR.CalibrationDialogTitle))
			{
				return;
			}

			double lengthInMm = component.LengthInCm * 10;
			ApplyCalibration(lengthInMm);
		}

		private double GetCurrentLength()
		{
			Units units = Units.Centimeters;

			IPointsGraphic line = LineGraphic;
			line.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				double length = RoiLengthAnalyzer.CalculateLength(line.Points[0], line.Points[1],
									Frame.NormalizedPixelSpacing, ref units);

				if (units == Units.Centimeters)
					return length;
				else
					return 0.0;
			}
			finally
			{
				line.ResetCoordinateSystem();
			}
		}

		private void ApplyCalibration(double lengthInMm)
		{
			ApplyCalibration(lengthInMm, LineGraphic, Frame, Context.DesktopWindow);
		}

		#if	UNIT_TESTS

		internal static void TestCalibration(double lengthInMm, IPointsGraphic graphic)
		{
			Frame frame = ((IImageSopProvider)graphic.ParentPresentationImage).Frame;
			ApplyCalibration(lengthInMm, graphic, frame, null);
		}

		#endif

		private static void ApplyCalibration(double lengthInMm, IPointsGraphic line, Frame frame, IDesktopWindow desktopWindow)
		{
			double aspectRatio;
			
			if (frame.PixelAspectRatio.IsNull)
			{
				// When there is no aspect ratio tag, the image is displayed with the aspect ratio
				// of the pixel spacing, so just keep the aspect ratio the same as
				// what's displayed.  Otherwise, after calibration, a 2cm line drawn horizontally
				// would be visibly different from a 2cm line drawn vertically.
				if (!frame.NormalizedPixelSpacing.IsNull)
					aspectRatio = frame.NormalizedPixelSpacing.AspectRatio;
				else
					aspectRatio = 1;
			}
			else
			{
				aspectRatio = frame.PixelAspectRatio.Value;
			}

			line.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = line.Points[1].X - line.Points[0].X;
			double heightInPixels = line.Points[1].Y - line.Points[0].Y;
			line.ResetCoordinateSystem();

			if (widthInPixels == 0 && heightInPixels == 0)
			{
				desktopWindow.ShowMessageBox(SR.ErrorCannotCalibrateZeroLengthRuler, MessageBoxActions.Ok);
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

			frame.NormalizedPixelSpacing.Calibrate(pixelSpacingHeight, pixelSpacingWidth);
			line.ParentPresentationImage.Draw();
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
