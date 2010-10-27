#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Allows Renderers to publish the total time elapsed for a particular method to aid in 
	/// debugging and optimization.
	/// </summary>
	[Obsolete("Use PerformanceReportBroker instead.")]
	public static class RenderPerformanceReportBroker
	{
		/// <summary>
		/// A Delegate for publishing performance of a method.
		/// </summary>
		public delegate void PerformanceReportDelegate(string methodName, double totalTime);

		/// <summary>
		/// A Delegate that can be subscribed to in order to receive performance reports.
		/// </summary>
		public static event PerformanceReportDelegate PerformanceReport;

		/// <summary>
		/// Called from within a method to publish performance reports to subscribers.
		/// </summary>
		public static void PublishPerformanceReport(string methodName, double totalTime)
		{
			if (PerformanceReport == null)
				return;

			PerformanceReport(methodName, totalTime);
		}
	}
}
