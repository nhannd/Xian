using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[ExtensionOf(typeof(ProtractorAnalyzerExtensionPoint))]
	public class ProtractorAngleCalculator : IRoiAnalyzer<PolyLineInteractiveGraphic>
	{
		public string Analyze(PolyLineInteractiveGraphic protractor)
		{
			// Don't show the callout until the second ray drawn
			if (protractor.PolyLine.Count < 3)
				return "Set Vertex";

			protractor.CoordinateSystem = CoordinateSystem.Destination;

			double angle = Formula.SubtendedAngle(
				protractor.PolyLine[0],
				protractor.PolyLine[1],
				protractor.PolyLine[2]);

			protractor.ResetCoordinateSystem();
			string text = String.Format("{0:F0}°", angle);

			return text;
		}
	}
}
