using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class ProtractorAnalyzer : IProtractorAnalyzer
	{
		public string Analyze(RoiGraphic roiGraphic)
		{
			PolyLineInteractiveGraphic protractor = roiGraphic.Roi as PolyLineInteractiveGraphic;
			IImageSopProvider provider = roiGraphic.ParentPresentationImage as IImageSopProvider;

			Platform.CheckForInvalidCast(protractor, "roiGraphic.Roi", "PolyLineInteractiveGraphic");

			return Analyze(provider.ImageSop, protractor);
		}

		public abstract string Analyze(ImageSop sop, PolyLineInteractiveGraphic rectangle);
	}

	[ExtensionOf(typeof(ProtractorAnalyzerExtensionPoint))]
	public class ProtractorAngleCalculator : ProtractorAnalyzer
	{
		public override string Analyze(ImageSop imageSop, PolyLineInteractiveGraphic protractor)
		{
			// Don't show the callout until the second ray drawn
			if (protractor.PolyLine.Count < 3)
				return "Set Vertex";

			protractor.CoordinateSystem = CoordinateSystem.Destination;

			double angle = Vector.SubtendedAngle(
				protractor.PolyLine[0],
				protractor.PolyLine[1],
				protractor.PolyLine[2]);

			protractor.ResetCoordinateSystem();
			string text = String.Format("{0:F2}°", angle);

			return text;
		}
	}
}
