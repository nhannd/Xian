using System;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class EllipseAnalyzer : IEllipseAnalyzer
	{
		public string Analyze(RoiGraphic roiGraphic)
		{
			EllipseInteractiveGraphic ellipse = roiGraphic.Roi as EllipseInteractiveGraphic;
			Platform.CheckForInvalidCast(ellipse, "roiGraphic.Roi", "EllipseInteractiveGraphic");

			return Analyze(ellipse);
		}

		public abstract string Analyze(EllipseInteractiveGraphic ellipse);
	}

	[ExtensionOf(typeof(EllipseAnalyzerExtensionPoint))]
	public class EllipseAreaCalculator : EllipseAnalyzer
	{
		public override string Analyze(EllipseInteractiveGraphic ellipse)
		{
			IImageSopProvider provider = ellipse.ParentPresentationImage as IImageSopProvider;
			ImageSop imageSop = provider.ImageSop;

			Units units = Units.Centimeters;

			ellipse.CoordinateSystem = CoordinateSystem.Source;
			double areaInPixels = Formula.AreaOfEllipse(ellipse.Width, ellipse.Height);
			ellipse.ResetCoordinateSystem();

			PixelSpacing pixelSpacing = imageSop.GetModalityPixelSpacing();

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
	public class EllipseStatisticsCalculator : EllipseAnalyzer
	{
		float a, b, a2, b2, h, k, xh, yk, r;

		public override string Analyze(EllipseInteractiveGraphic ellipse)
		{
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
