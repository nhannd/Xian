using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class RectangleAnalyzer : IRectangleAnalyzer
	{
		public string Analyze(RoiGraphic roiGraphic)
		{
			RectangleInteractiveGraphic rectangle = roiGraphic.Roi as RectangleInteractiveGraphic;
			IImageSopProvider provider = roiGraphic.ParentPresentationImage as IImageSopProvider;
			
			Platform.CheckForInvalidCast(rectangle, "roiGraphic.Roi", "RectangleInteractiveGraphic");
			
			return Analyze(provider.ImageSop, rectangle);
		}

		public abstract string Analyze(ImageSop sop, RectangleInteractiveGraphic rectangle);
	}

	[ExtensionOf(typeof(RectangleAnalyzerExtensionPoint))]
	public class RectangleAreaCalculator : RectangleAnalyzer
	{
		public override string Analyze(ImageSop imageSop, RectangleInteractiveGraphic rectangle)
		{
			Units units = Units.Centimeters;

			rectangle.CoordinateSystem = CoordinateSystem.Source;
			double areaInPixels = Formula.AreaOfRectangle(rectangle.Width, rectangle.Height);
			rectangle.ResetCoordinateSystem();

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
