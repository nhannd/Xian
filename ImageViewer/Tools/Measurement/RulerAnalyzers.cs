using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(RulerAnalyzerExtensionPoint))]
	public class RulerLengthCalculator : IRoiAnalyzer<PolyLineInteractiveGraphic>
	{
		public string Analyze(PolyLineInteractiveGraphic line, RoiAnalysisMethod method)
		{
			IImageSopProvider provider = line.ParentPresentationImage as IImageSopProvider;

			if (provider == null)
				return String.Empty;

			ImageSop imageSop = provider.ImageSop;
			Units units = Units.Centimeters;
			PixelSpacing pixelSpacing = imageSop.NormalizedPixelSpacing;

			string text;

			double length = CalculateLength(line, pixelSpacing, ref units);

			if (units == Units.Pixels)
				text = String.Format(SR.ToolsMeasurementFormatLengthPixels, length);
			else if (units == Units.Millimeters)
				text = String.Format(SR.ToolsMeasurementFormatLengthMm, length);
			else
				text = String.Format(SR.ToolsMeasurementFormatLengthCm, length);

			return text;
		}

		public static double CalculateLength(
			PolyLineInteractiveGraphic line, 
			PixelSpacing pixelSpacing, 
			ref Units units)
		{
			if (pixelSpacing.IsNull)
				units = Units.Pixels;

			line.CoordinateSystem = CoordinateSystem.Source;
			double widthInPixels = line.PolyLine[1].X - line.PolyLine[0].X;
			double heightInPixels = line.PolyLine[1].Y - line.PolyLine[0].Y;
			line.ResetCoordinateSystem();

			double length;

			if (units == Units.Pixels)
			{
				length = Math.Sqrt(widthInPixels * widthInPixels + heightInPixels * heightInPixels);
			}
			else
			{
				double widthInMm = widthInPixels * pixelSpacing.Column;
				double heightInMm = heightInPixels * pixelSpacing.Row;
				double lengthInMm = Math.Sqrt(widthInMm * widthInMm + heightInMm * heightInMm);

				if (units == Units.Millimeters)
					length = lengthInMm;
				else
					length = lengthInMm / 10;
			}

			return length;
		}
	}
}
