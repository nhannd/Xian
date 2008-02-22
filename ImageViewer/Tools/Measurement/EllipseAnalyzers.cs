using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(RoiAnalyzerExtensionPoint<EllipseRoiInfo>))]
	public class EllipseAreaCalculator : IRoiAnalyzer<EllipseRoiInfo>
	{
		public string Analyze(EllipseRoiInfo roiInfo)
		{
			Units units = Units.Centimeters;

			double areaInPixels = Formula.AreaOfEllipse(roiInfo.BoundingBox.Width, roiInfo.BoundingBox.Height);

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

	[ExtensionOf(typeof(RoiAnalyzerExtensionPoint<EllipseRoiInfo>))]
	public class EllipseStatisticsCalculator : IRoiAnalyzer<EllipseRoiInfo>
	{
		float a, b, a2, b2, h, k, xh, yk, r;

		public string Analyze(EllipseRoiInfo roiInfo)
		{
			RectangleF boundingBox = roiInfo.BoundingBox;

			a = boundingBox.Width / 2;
			b = boundingBox.Height / 2;
			a2 = a * a;
			b2 = b * b;
			h = boundingBox.Left + a;
			k = boundingBox.Top + b;

			return RoiStatisticsCalculator.Calculate(roiInfo, IsPointInRoi);
		}

		public bool IsPointInRoi(int x, int y)
		{
			xh = x - h;
			yk = y - k;
			r = (xh * xh /a2) + (yk * yk /b2);

			if (r <= 1)
				return true;
			else
				return false;
		}
	}
}
