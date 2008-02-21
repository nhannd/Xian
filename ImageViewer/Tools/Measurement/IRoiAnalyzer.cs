using System;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public enum RoiAnalysisMethod
	{
		Accurate,

		Fast
	}

	public interface IRoiAnalyzer<T> where T:InteractiveGraphic
	{
		string Analyze(T roiGraphic, RoiAnalysisMethod method);
	}
}
