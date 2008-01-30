using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class RulerAnalyzer : IRulerAnalyzer
	{
		public string Analyze(RoiGraphic roiGraphic)
		{
			PolyLineInteractiveGraphic ruler = roiGraphic.Roi as PolyLineInteractiveGraphic;

			Platform.CheckForInvalidCast(ruler, "roiGraphic.Roi", "PolyLineInteractiveGraphic");

			return Analyze(ruler);
		}

		public abstract string Analyze(PolyLineInteractiveGraphic rectangle);
	}

	[ExtensionOf(typeof(RulerAnalyzerExtensionPoint))]
	public class RulerLengthCalculator : RulerAnalyzer
	{
		public override string Analyze(PolyLineInteractiveGraphic line)
		{
			IImageSopProvider provider = line.ParentPresentationImage as IImageSopProvider;
			ImageSop imageSop = provider.ImageSop;

			Units units = Units.Centimeters;

			line.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = line.PolyLine[1].X - line.PolyLine[0].X;
			double heightInPixels = line.PolyLine[1].Y - line.PolyLine[0].Y;
			line.ResetCoordinateSystem();

			PixelSpacing pixelSpacing = imageSop.GetModalityPixelSpacing();

			string text;

			if (pixelSpacing.IsNull || units == Units.Pixels)
			{
				double length = Math.Sqrt(widthInPixels * widthInPixels + heightInPixels * heightInPixels);
				text = String.Format(SR.ToolsMeasurementFormatLengthPixels, length);
			}
			else
			{
				double widthInMm = widthInPixels * pixelSpacing.Column;
				double heightInMm = heightInPixels * pixelSpacing.Row;
				double lengthInMm = Math.Sqrt(widthInMm * widthInMm + heightInMm * heightInMm);

				if (units == Units.Millimeters)
					text = String.Format(SR.ToolsMeasurementFormatLengthMm, lengthInMm);
				else
					text = String.Format(SR.ToolsMeasurementFormatLengthCm, lengthInMm / 10);
			}
			return text;
		}
	}
}
