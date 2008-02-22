using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(RoiAnalyzerExtensionPoint<ProtractorRoiInfo>))]
	public class ProtractorAngleCalculator : IRoiAnalyzer<ProtractorRoiInfo>
	{
		public string Analyze(ProtractorRoiInfo roiInfo)
		{
			// Don't show the callout until the second ray is drawn
			if (roiInfo.Points.Count < 3)
				return SR.ToolsMeasurementSetVertex;

			double angle = Formula.SubtendedAngle(
				roiInfo.Points[0],
				roiInfo.Points[1],
				roiInfo.Points[2]);

			return String.Format(SR.ToolsMeasurementFormatDegrees, Math.Abs(angle));
		}
	}
}
