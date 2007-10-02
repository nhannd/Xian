using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Allows Renderers to publish the total time elapsed for a particular method to aid in 
	/// debugging and optimization.
	/// </summary>
	public static class RenderPerformanceReportBroker
	{
		/// <summary>
		/// A Delegate for publishing performance of a method.
		/// </summary>
		public delegate void PerformanceReportDelegate(string methodName, double totalTime);

		/// <summary>
		/// A Delegate that can be subscribed to in order to receive performance reports.
		/// </summary>
		public static PerformanceReportDelegate PerformanceReport;

		/// <summary>
		/// Called from within a method to publish performance reports to subscribers.
		/// </summary>
		public static void PublishPerformanceReport(string methodName, double totalTime)
		{
			if (PerformanceReport == null)
				return;

			EventsHelper.Fire(PerformanceReport, methodName, totalTime);
		}
	}

}
