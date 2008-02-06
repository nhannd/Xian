using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public interface IRoiAnalyzer<T> where T:InteractiveGraphic
	{
		string Analyze(T roiGraphic);
	}
}
