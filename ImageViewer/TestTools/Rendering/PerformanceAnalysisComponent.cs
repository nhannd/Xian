#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using System;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.TestTools.Rendering
{
	[ExtensionPoint]
	public sealed class PerformanceAnalysisComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(PerformanceAnalysisComponentViewExtensionPoint))]
	public class PerformanceAnalysisComponent : ImageViewerToolComponent
	{
		public class ReportItem
		{
			internal ReportItem(string reportBrokerName, int numberOfCalls, TimeSpan cumulativeTime)
			{
				ReportBrokerName = reportBrokerName;
				NumberOfCalls = numberOfCalls;
				CumulativeTime = cumulativeTime;
			}

			public readonly string ReportBrokerName;
			public readonly int NumberOfCalls;
			public readonly TimeSpan CumulativeTime;

			public ReportItem Increment(double seconds)
			{
				return new ReportItem(ReportBrokerName, NumberOfCalls + 1, CumulativeTime + TimeSpan.FromSeconds(seconds));
			}
		}
		
		private Table<ReportItem> _reportTable;
		private SimpleActionModel _menuModel;
		private string _description;

		public PerformanceAnalysisComponent()
			: base(Application.ActiveDesktopWindow)
		{
		}

		public ITable Table
		{
			get { return _reportTable; }	
		}

		public ActionModelNode MenuModel
		{
			get { return _menuModel; }	
		}

		public string Description
		{
			get { return _description; }	
			set
			{
				_description = value;
				NotifyPropertyChanged("Description");
			}
		}

		public override void Start()
		{
			base.Start();
			
			RenderPerformanceReportBroker.PerformanceReport += OnReceivedReport;

			_reportTable = new Table<ReportItem>();
			_reportTable.Columns.Add(new TableColumn<ReportItem, string>("Name", delegate(ReportItem item) { return item.ReportBrokerName; }));
			_reportTable.Columns.Add(new TableColumn<ReportItem, int>("#Calls", delegate(ReportItem item) { return item.NumberOfCalls; }));
			_reportTable.Columns.Add(new TableColumn<ReportItem, string>("Time", delegate(ReportItem item) { return item.CumulativeTime.ToString(); }));

			_menuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			_menuModel.AddAction("reset", "Reset", null, null, Reset);
		}

		public override void Stop()
		{
			RenderPerformanceReportBroker.PerformanceReport -= OnReceivedReport;

			base.Stop();
		}

		public void Reset()
		{
			_reportTable.Items.Clear();
		}

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			base.OnActiveImageViewerChanged(e);
			if (e.DeactivatedImageViewer != null)
				e.DeactivatedImageViewer.PhysicalWorkspace.ScreenRectangleChanged -= new EventHandler(OnScreenRectangleChanged);

			if (e.ActivatedImageViewer != null)
			{
				e.ActivatedImageViewer.PhysicalWorkspace.ScreenRectangleChanged += new EventHandler(OnScreenRectangleChanged);
				OnScreenRectangleChanged(null, EventArgs.Empty);
			}
		}

		private void OnScreenRectangleChanged(object sender, EventArgs e)
		{
			if (base.ImageViewer == null)
				this.Description = "n/a";
			else
				this.Description = String.Format("{0}x{1}", base.ImageViewer.PhysicalWorkspace.ScreenRectangle.Width,
					              base.ImageViewer.PhysicalWorkspace.ScreenRectangle.Height);
		}

		private void OnReceivedReport(string methodName, double totalTime)
		{
			ReportItem item = CollectionUtils.SelectFirst(_reportTable.Items, 
				delegate(ReportItem test) { return test.ReportBrokerName == methodName; });

			if (item == null)
			{
				item = new ReportItem(methodName, 1, TimeSpan.FromSeconds(totalTime));
				_reportTable.Items.Add(item);
			}
			else 
			{
				_reportTable.Items[_reportTable.Items.IndexOf(item)] = item.Increment(totalTime);
			}
		}
	}
}
