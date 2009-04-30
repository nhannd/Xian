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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using System;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;
using System.Threading;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ExtensionPoint]
	public sealed class PerformanceAnalysisComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(PerformanceAnalysisComponentViewExtensionPoint))]
	public class PerformanceAnalysisComponent : ApplicationComponent
	{
		public class ReportItem
		{
			public ReportItem(string category, string identifier, int numberOfCalls, TimeSpan cumulativeTime)
			{
				Category = category;
				Identifier = identifier;
				NumberOfCalls = numberOfCalls;
				CumulativeTime = cumulativeTime;
			}

			public readonly string Category;
			public readonly string Identifier;
			public readonly int NumberOfCalls;
			public readonly TimeSpan CumulativeTime;

			public ReportItem Increment(TimeSpan seconds)
			{
				return new ReportItem(Category, Identifier, NumberOfCalls + 1, CumulativeTime + seconds);
			}
		}
		
		private Table<ReportItem> _reportTable;
		private SimpleActionModel _menuModel;
		private string _description;
		private volatile SynchronizationContext _uiThreadContext;

		public PerformanceAnalysisComponent()
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

		public override void Start()
		{
			base.Start();

			_uiThreadContext = SynchronizationContext.Current;

			_reportTable = new Table<ReportItem>();
			_reportTable.Columns.Add(new TableColumn<ReportItem, string>("Category", delegate(ReportItem item) { return item.Category; }));
			_reportTable.Columns.Add(new TableColumn<ReportItem, string>("Identifier", delegate(ReportItem item) { return item.Identifier; }));
			_reportTable.Columns.Add(new TableColumn<ReportItem, int>("#Calls", delegate(ReportItem item) { return item.NumberOfCalls; }));
			_reportTable.Columns.Add(new TableColumn<ReportItem, string>("Time", delegate(ReportItem item) { return item.CumulativeTime.ToString(); }));

			_menuModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));
			_menuModel.AddAction("reset", "Reset", null, null, Reset);

			PerformanceReportBroker.Report += OnReceivedReport;
		}

		public override void Stop()
		{
			PerformanceReportBroker.Report -= OnReceivedReport;
			_uiThreadContext = null;
			
			base.Stop();
		}

		public void Reset()
		{
			_reportTable.Items.Clear();
		}

		private void OnReceivedReport(object sender, ItemEventArgs<PerformanceReport> report)
		{
			if (_uiThreadContext == null)
				return;

			if (_uiThreadContext != SynchronizationContext.Current)
			{
				_uiThreadContext.Post(delegate { OnReceivedReport(sender, report); }, null);
				return;
			}

			ReportItem item = CollectionUtils.SelectFirst(_reportTable.Items, 
				delegate(ReportItem test)
					{
						return report.Item.Category == test.Category && test.Identifier == report.Item.Identifier;
					});

			if (item == null)
			{
				item = new ReportItem(report.Item.Category, report.Item.Identifier, 1, report.Item.TotalTime);
				_reportTable.Items.Add(item);
			}
			else 
			{
				int index = _reportTable.Items.IndexOf(item);
				ReportItem newItem = item.Increment(report.Item.TotalTime);
				_reportTable.Items[index] = newItem;
				_reportTable.Items.NotifyItemUpdated(index);
			}
		}
	}
}
