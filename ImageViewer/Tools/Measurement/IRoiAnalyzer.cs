using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public enum RoiAnalysisMode
	{
		Normal = 0,

		Responsive
	}

	public interface IRoiAnalyzer<T> where T : RoiInfo
	{
		string Analyze(T roiInfo);
	}

	public sealed class RoiAnalyzerExtensionPoint<T> : ExtensionPoint<IRoiAnalyzer<T>> where T : RoiInfo
	{
		public RoiAnalyzerExtensionPoint()
		{
		}
	}
}
