using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(EllipseAnalyzerExtensionPoint))]
	public class EllipseAreaCalculator : IRoiAnalyzer<EllipseInteractiveGraphic>
	{
		public string Analyze(EllipseInteractiveGraphic ellipse, RoiAnalysisMethod method)
		{
			IImageSopProvider provider = ellipse.ParentPresentationImage as IImageSopProvider;

			if (provider == null)
				return String.Empty;
			
			ImageSop imageSop = provider.ImageSop;

			Units units = Units.Centimeters;

			ellipse.CoordinateSystem = CoordinateSystem.Source;
			double areaInPixels = Formula.AreaOfEllipse(ellipse.Width, ellipse.Height);
			ellipse.ResetCoordinateSystem();

			PixelSpacing pixelSpacing = imageSop.NormalizedPixelSpacing;

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

	[ExtensionOf(typeof(EllipseAnalyzerExtensionPoint))]
	public class EllipseStatisticsCalculator : IRoiAnalyzer<EllipseInteractiveGraphic>
	{
		float a, b, a2, b2, h, k, xh, yk, r;

		public string Analyze(EllipseInteractiveGraphic ellipse, RoiAnalysisMethod method)
		{
			if (method == RoiAnalysisMethod.Fast)
			{
				return String.Format("{0} {1}\n{2} {1}", SR.ToolsMeasurementMean, SR.ToolsMeasurementCalculating, SR.ToolsMeasurementStdev);
			}

			ellipse.CoordinateSystem = CoordinateSystem.Source;

			a = ellipse.Width/2;
			b = ellipse.Height/2;
			a2 = a*a;
			b2 = b*b;
			h = ellipse.Left + a;
			k = ellipse.Top + b;

			string str = RoiStatisticsCalculator.Calculate(ellipse, IsPointInRoi);

			ellipse.ResetCoordinateSystem();

			return str;
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
