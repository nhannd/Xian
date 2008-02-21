using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(RectangleAnalyzerExtensionPoint))]
	public class RectangleAreaCalculator : IRoiAnalyzer<RectangleInteractiveGraphic>
	{
		public string Analyze(RectangleInteractiveGraphic rectangle, RoiAnalysisMethod method)
		{
			IImageSopProvider provider = rectangle.ParentPresentationImage as IImageSopProvider;

			if (provider == null)
				return String.Empty;
			
			ImageSop imageSop = provider.ImageSop;

			Units units = Units.Centimeters;

			rectangle.CoordinateSystem = CoordinateSystem.Source;
			double areaInPixels = Formula.AreaOfRectangle(rectangle.Width, rectangle.Height);
			rectangle.ResetCoordinateSystem();

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

	[ExtensionOf(typeof(RectangleAnalyzerExtensionPoint))]
	public class RectangleStatisticsCalculator : IRoiAnalyzer<RectangleInteractiveGraphic>
	{
		public string Analyze(RectangleInteractiveGraphic rectangle, RoiAnalysisMethod method)
		{
			if (method == RoiAnalysisMethod.Fast)
			{
				return String.Format("{0} {1}\n{2} {1}", SR.ToolsMeasurementMean, SR.ToolsMeasurementCalculating, SR.ToolsMeasurementStdev);
			}

			rectangle.CoordinateSystem = CoordinateSystem.Source;

			string str = RoiStatisticsCalculator.Calculate(rectangle, IsPointInRoi);

			rectangle.ResetCoordinateSystem();

			return str;
		}

		public bool IsPointInRoi(int x, int y)
		{
			return true;
		}
	}
}
