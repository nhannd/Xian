using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(RoiAnalyzerExtensionPoint<RulerRoiInfo>))]
	public class RulerLengthCalculator : IRoiAnalyzer<RulerRoiInfo>
	{
		public string Analyze(RulerRoiInfo roiInfo)
		{
			Units units = Units.Centimeters;

			string text;

			double length = CalculateLength(roiInfo.Point1, roiInfo.Point2, roiInfo.NormalizedPixelSpacing, ref units);

			if (units == Units.Pixels)
				text = String.Format(SR.ToolsMeasurementFormatLengthPixels, length);
			else if (units == Units.Millimeters)
				text = String.Format(SR.ToolsMeasurementFormatLengthMm, length);
			else
				text = String.Format(SR.ToolsMeasurementFormatLengthCm, length);

			return text;
		}

		public static double CalculateLength(
			PointF point1,
			PointF point2,
			PixelSpacing normalizedPixelSpacing,
			ref Units units)
		{
			if (normalizedPixelSpacing.IsNull)
				units = Units.Pixels;

			double widthInPixels = point2.X - point1.X;
			double heightInPixels = point2.Y - point1.Y;

			double length;

			if (units == Units.Pixels)
			{
				length = Math.Sqrt(widthInPixels * widthInPixels + heightInPixels * heightInPixels);
			}
			else
			{
				double widthInMm = widthInPixels * normalizedPixelSpacing.Column;
				double heightInMm = heightInPixels * normalizedPixelSpacing.Row;
				double lengthInMm = Math.Sqrt(widthInMm * widthInMm + heightInMm * heightInMm);

				if (units == Units.Millimeters)
					length = lengthInMm;
				else
					length = lengthInMm / 10;
			}

			return length;
		}

		public static double CalculateLength(
			PolyLineInteractiveGraphic polyLineInteractiveGraphic,
			PixelSpacing normalizedPixelSpacing,
			ref Units units)
		{
			return CalculateLength(
				polyLineInteractiveGraphic.PolyLine[0],
				polyLineInteractiveGraphic.PolyLine[1],
				normalizedPixelSpacing, ref units);
		}
	}
}
