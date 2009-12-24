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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using System;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="PriorReportComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class PriorReportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PriorReportComponent class
	/// </summary>
	[AssociateView(typeof(PriorReportComponentViewExtensionPoint))]
	public class PriorReportComponent : ApplicationComponent
	{
		class ReportViewComponent : DHtmlComponent
		{
			private readonly PriorReportComponent _owner;

			public ReportViewComponent(PriorReportComponent owner)
			{
				_owner = owner;
			}

			public override void Start()
			{
				SetUrl(WebResourcesSettings.Default.PriorReportPreviewPageUrl);
				base.Start();
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _owner._selectedPrior;
			}
		}

		private ReportingWorklistItem _worklistItem;

		private readonly PriorSummaryTable _reportList;
		private PriorProcedureSummary _selectedPrior;

		private bool _relevantPriorsOnly;

		private List<PriorProcedureSummary> _relevantPriors;
		private List<PriorProcedureSummary> _allPriors;

		private ChildComponentHost _reportViewComponentHost;

		/// <summary>
		/// Constructor for showing priors based on a reporting step.
		/// </summary>
		public PriorReportComponent(ReportingWorklistItem worklistItem)
		{
			_worklistItem = worklistItem;
			_relevantPriorsOnly = PriorReportComponentSettings.Default.ShowRelevantPriorsOnly;

			_reportList = new PriorSummaryTable();
		}

		public override void Start()
		{
			_reportViewComponentHost = new ChildComponentHost(this.Host, new ReportViewComponent(this));
			_reportViewComponentHost.StartComponent();

			UpdateReportList();

			base.Start();
		}

		public override void Stop()
		{
			if (_reportViewComponentHost != null)
			{
				_reportViewComponentHost.StopComponent();
				_reportViewComponentHost = null;
			}

			base.Stop();
		}

		#region Presentation Model

		public ApplicationComponentHost ReportViewComponentHost
		{
			get { return _reportViewComponentHost; }
		}

		public bool RelevantPriorsOnly
		{
			get { return _relevantPriorsOnly; }
			set
			{
				if (value != _relevantPriorsOnly)
				{
					_relevantPriorsOnly = value;
					PriorReportComponentSettings.Default.ShowRelevantPriorsOnly = value;
					PriorReportComponentSettings.Default.Save();
					UpdateReportList();
				}
			}
		}

		public bool AllPriors
		{
			get { return !this.RelevantPriorsOnly; }
		}

		public ITable Reports
		{
			get { return _reportList; }
		}

		public ISelection SelectedReport
		{
			get { return new Selection(_selectedPrior); }
			set
			{
				var newSelection = (PriorProcedureSummary)value.Item;
				if (_selectedPrior != newSelection)
				{
					_selectedPrior = newSelection;
					((ReportViewComponent)_reportViewComponentHost.Component).Refresh();
				}
			}
		}

		#endregion

		private void UpdateReportList()
		{
			// cancel any pending requests
			Async.CancelPending(this);

			// clear table
			_reportList.Items.Clear();

			if (_relevantPriorsOnly)
			{
				Async.Invoke(this,
					delegate
					{
						if (_relevantPriors == null)
							_relevantPriors = LoadPriors(true);
					},
					delegate
					{
						_reportList.Items.AddRange(_relevantPriors);
					});

			}
			else
			{
				Async.Invoke(this,
					delegate
					{
						if (_allPriors == null)
							_allPriors = LoadPriors(false);
					},
					delegate
					{
						_reportList.Items.AddRange(_allPriors);
					});
			}
		}

		private List<PriorProcedureSummary> LoadPriors(bool relevantOnly)
		{
			GetPriorsResponse response = null;
			Platform.GetService<IReportingWorkflowService>(service =>
				{
					var request = new GetPriorsRequest();
					if (relevantOnly)
					{
						if (_worklistItem.ReportRef != null)
							request.ReportRef = _worklistItem.ReportRef;
						else
							request.OrderRef = _worklistItem.OrderRef;
					}
					else
						request.PatientRef = _worklistItem.PatientRef;
					response = service.GetPriors(request);
				});
			return response.Reports;
		}

		public ReportingWorklistItem WorklistItem
		{
			get { return _worklistItem; }
			set
			{
				_worklistItem = value;

				if (this.IsStarted)
				{
					_relevantPriors = null;
					_allPriors = null;
					UpdateReportList();
					((ReportViewComponent)_reportViewComponentHost.Component).Refresh();
				}
			}
		}
	}
}
