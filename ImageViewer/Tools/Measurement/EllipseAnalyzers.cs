using System;
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
			IImageSopProvider provider = roiGraphic.ParentPresentationImage as IImageSopProvider;

			Platform.CheckForInvalidCast(ellipse, "roiGraphic.Roi", "EllipseInteractiveGraphic");

			return Analyze(provider.ImageSop, ellipse);
		}

		public abstract string Analyze(ImageSop sop, EllipseInteractiveGraphic rectangle);
	}

	[ExtensionOf(typeof(EllipseAnalyzerExtensionPoint))]
	public class EllipseAreaCalculator : EllipseAnalyzer
	{
		public override string Analyze(ImageSop imageSop, EllipseInteractiveGraphic ellipse)
		{
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
}
