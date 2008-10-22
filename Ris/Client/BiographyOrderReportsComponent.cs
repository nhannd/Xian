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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyOrderReportsComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class BiographyOrderReportsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[ExtensionPoint]
	public class BiographyOrderReportsToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IBiographyOrderReportsToolContext : IToolContext
	{
		EntityRef PatientRef { get; }
		EntityRef PatientProfileRef { get; }
		EntityRef OrderRef { get; }
		EntityRef ReportRef { get; }
		EnumValueInfo ReportStatus { get; }
		IDesktopWindow DesktopWindow { get; }
		event EventHandler ContextChanged;
	}

	/// <summary>
	/// BiographyOrderReportsComponent class.
	/// </summary>
	[AssociateView(typeof(BiographyOrderReportsComponentViewExtensionPoint))]
	public class BiographyOrderReportsComponent : ApplicationComponent
	{
		public class BiographyOrderReportsToolContext : ToolContext, IBiographyOrderReportsToolContext
		{
			private readonly BiographyOrderReportsComponent _component;

			internal BiographyOrderReportsToolContext(BiographyOrderReportsComponent component)
			{
				_component = component;
			}

			#region IBiographyOrderReportsToolContext Members

			public EntityRef PatientRef
			{
				get { return _component.PatientRef; }
			}

			public EntityRef PatientProfileRef
			{
				get { return _component.PatientProfileRef; }
			}

			public EntityRef OrderRef
			{
				get { return _component.OrderRef; }
			}

			public EntityRef ReportRef
			{
				get { return _component.ReportRef; }
			}

			public EnumValueInfo ReportStatus
			{
				get { return _component.ReportStatus; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public event EventHandler ContextChanged
			{
				add { _component.ReportSelectionChanged += value; }
				remove { _component.ReportSelectionChanged -= value; }
			}

			#endregion
		}

		class ReportPreviewComponent : DHtmlComponent
		{
			// Internal data contract used for jscript deserialization
			[DataContract]
			public class ReportPreviewContext : DataContractBase
			{
				public ReportPreviewContext(EntityRef reportRef)
				{
					this.ReportRef = reportRef;
				}

				[DataMember]
				public EntityRef ReportRef;
			}

			private ReportPreviewContext _previewContext;

			public ReportPreviewComponent(EntityRef reportRef)
			{
				_previewContext = reportRef != null ? new ReportPreviewContext(reportRef) : null;
			}

			public override void Start()
			{
				SetUrl(WebResourcesSettings.Default.BiographyReportDetailPageUrl);
				base.Start();
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _previewContext;
			}

			public ReportPreviewContext PreviewContext
			{
				get { return _previewContext; }
				set
				{
					_previewContext = value;
					Refresh();
				}
			}
		}

		public class ReportsContext
		{
			public ReportsContext(EntityRef orderRef, EntityRef patientRef)
			{
				OrderRef = orderRef;
				PatientRef = patientRef;
			}

			public EntityRef OrderRef;
			public EntityRef PatientRef;
		}

		/// <summary>
		/// Represents a collection of <see cref="ReportListItem"/> which share a common report which should be presented to the user
		/// as a single entity.
		/// </summary>
		public class CommonReportListItem
		{
			private readonly EntityRef _reportRef;
			private readonly IList<ReportListItem> _reportListItems;

			public CommonReportListItem(EntityRef reportRef, ReportListItem firstItem)
			{
				_reportRef = reportRef;
				_reportListItems = new List<ReportListItem>();
				_reportListItems.Add(firstItem);
			}

			public EntityRef ReportRef
			{
				get { return _reportRef; }
			}

			public EnumValueInfo ReportStatus
			{
				get { return CollectionUtils.FirstElement(_reportListItems).ReportStatus; }
			}

			public void AddReportListItem(ReportListItem item)
			{
				if(item != null)
					_reportListItems.Add(item);
			}

			public override string ToString()
			{
				if (_reportListItems.Count == 0)
					return string.Empty;
				
				string s = string.Join(" / ",
					CollectionUtils.Map<ReportListItem, string>(
						_reportListItems, 
						delegate (ReportListItem item)
						{
							return ProcedureFormat.Format(item);
						}).ToArray());

				s += " : " + _reportListItems[0].ReportStatus.Value;

				return s;
			}
		}

		private ReportsContext _context;

		private List<CommonReportListItem> _reports;
		private CommonReportListItem _selectedReport;

		private ToolSet _toolSet;

		private ReportPreviewComponent _reportPreviewComponent;
		private ChildComponentHost _reportPreviewComponentHost;
		private EntityRef _patientProfileRef;

		private event EventHandler _reportSelectionChanged;

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public BiographyOrderReportsComponent()
		{
			_context = null;
			_reports = new List<CommonReportListItem>();
		}

		#endregion

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_reportPreviewComponent = new ReportPreviewComponent(null);
			_reportPreviewComponentHost = new ChildComponentHost(this.Host, _reportPreviewComponent);
			_reportPreviewComponentHost.StartComponent();

			RefreshComponent();

			_toolSet = new ToolSet(new BiographyOrderReportsToolExtensionPoint(), new BiographyOrderReportsToolContext(this));

			base.Start();
		}

        public override void Stop()
        {
            if (_reportPreviewComponentHost != null)
            {
                _reportPreviewComponentHost.StopComponent();
                _reportPreviewComponentHost = null;
            }

            base.Stop();
        }

		#endregion

		#region Presentation model

		public IList Reports
		{
			get { return _reports; }
		}

		public CommonReportListItem SelectedReport
		{
			get { return _selectedReport; }
			set
			{
				if(!Equals(_selectedReport, value))
				{
					_selectedReport = value;
					OnReportSelectionChanged();
				}
			}
		}

		public event EventHandler ReportSelectionChanged
		{
			add { _reportSelectionChanged += value; }
			remove { _reportSelectionChanged -= value; }
		}

		public ActionModelNode ActionModel
		{
			get 
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "biography-reports-toolbar", _toolSet.Actions);
			}
		}

		public ApplicationComponentHost ReportPreviewComponentHost
		{
			get { return _reportPreviewComponentHost; }
		}

		public string FormatReportListItem(object item)
		{
			CommonReportListItem reportListItem = (CommonReportListItem)item;
			return reportListItem.ToString();
		}

		#endregion

		public ReportsContext Context
		{
			get { return _context; }
			set
			{
				_context = value;
				if(this.IsStarted)
				{
					RefreshComponent();
				}
			}
		}

		public EntityRef PatientRef
		{
			get { return this.Context != null ? this.Context.PatientRef : null; }
		}

		public EntityRef PatientProfileRef
		{
			get { return _patientProfileRef; }
		}

		public EntityRef OrderRef
		{
			get { return this.Context != null ? this.Context.OrderRef : null; }
		}

		public EntityRef ReportRef
		{
			get { return this._selectedReport != null ? this._selectedReport.ReportRef : null; }
		}

		public EnumValueInfo ReportStatus
		{
			get { return this._selectedReport != null ? this._selectedReport.ReportStatus : null; }
		}

		private void RefreshComponent()
		{
			if (_context == null)
			{
				_reports = new List<CommonReportListItem>();
				_selectedReport = null;
			}
			else
			{
				LoadReports();
			}

			OnReportSelectionChanged();
			NotifyAllPropertiesChanged();
		}

		private void LoadReports()
		{
			try
			{
				Platform.GetService<IBrowsePatientDataService>(
					delegate(IBrowsePatientDataService service)
					{
						GetDataRequest request = new GetDataRequest();
						request.ListReportsRequest = new ListReportsRequest(null, _context.OrderRef);
						request.ListPatientProfilesRequest = new ListPatientProfilesRequest(_context.PatientRef);
						request.GetOrderDetailRequest = new GetOrderDetailRequest(_context.OrderRef, false, true, false, false, false, false);

						GetDataResponse response = service.GetData(request);

						ProcedureDetail procedure = response.GetOrderDetailResponse.Order.Procedures[0];
						if (procedure != null)
						{
							string facilityCode = procedure.PerformingFacility.InformationAuthority.Code;
							PatientProfileSummary matchingProfile = CollectionUtils.SelectFirst(
								response.ListPatientProfilesResponse.Profiles,
								delegate(PatientProfileSummary summary)
								{
									return summary.Mrn.AssigningAuthority.Code == facilityCode;
								});
							_patientProfileRef = matchingProfile != null ? matchingProfile.PatientProfileRef : null;
						}


						List<CommonReportListItem> reports = new List<CommonReportListItem>();

						CollectionUtils.ForEach<ReportListItem>(
							response.ListReportsResponse.Reports,
							delegate(ReportListItem item)
							{
								CommonReportListItem existingItem = CollectionUtils.SelectFirst(
									reports,
									delegate(CommonReportListItem crli)
									{
										return Equals(crli.ReportRef, item.ReportRef);
									});

								if (existingItem != null)
								{
									existingItem.AddReportListItem(item);
								}
								else
								{
									reports.Add(new CommonReportListItem(item.ReportRef, item));
								}
							});

						_reports = reports;
						_selectedReport = CollectionUtils.FirstElement(_reports);
					});
			}
			catch (Exception e)
			{
				_reports = new List<CommonReportListItem>();
				_selectedReport = null;
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private void OnReportSelectionChanged()
		{
			_reportPreviewComponent.PreviewContext = _selectedReport != null ? new ReportPreviewComponent.ReportPreviewContext(_selectedReport.ReportRef) : null;
			EventsHelper.Fire(_reportSelectionChanged, this, EventArgs.Empty);
		}
	}
}