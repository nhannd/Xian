#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
