using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(RoiAnalyzerExtensionPoint<RectangularRoiInfo>))]
	public class RectangleAreaCalculator : IRoiAnalyzer<RectangularRoiInfo>
	{
		public string Analyze(RectangularRoiInfo roiInfo)
		{
			Units units = Units.Centimeters;

			double areaInPixels = Formula.AreaOfRectangle(roiInfo.BoundingBox.Width, roiInfo.BoundingBox.Height);

			PixelSpacing pixelSpacing = roiInfo.NormalizedPixelSpacing;

			string text;

			if (pixelSpacing.IsNull || units == Units.Pixels)
			{
				text = String.Format(SR.ToolsMeasurementFormatAreaPixels, areaInPixels);
			}
			else
			{
				double areaInMm = areaInPixels * pixelSpacing.Column * pixelSpacing.Row;

				if (units == Units.Millimeters)
					text = String.Format(SR.ToolsMeasurementFormatAreaSquareMm, areaInMm);
				else
					text = String.Format(SR.ToolsMeasurementFormatAreaSquareCm, areaInMm / 100);
			}
			return text;
		}
	}

	[ExtensionOf(typeof(RoiAnalyzerExtensionPoint<RectangularRoiInfo>))]
	public class RectangleStatisticsCalculator : IRoiAnalyzer<RectangularRoiInfo>
	{
		public string Analyze(RectangularRoiInfo roiInfo)
		{
			return RoiStatisticsCalculator.Calculate(roiInfo, IsPointInRoi);
		}

		public bool IsPointInRoi(int x, int y)
		{
			return true;
		}
	}
}
