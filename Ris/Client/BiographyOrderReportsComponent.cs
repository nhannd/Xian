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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyOrderReportsComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class BiographyOrderReportsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// BiographyOrderReportsComponent class.
	/// </summary>
	[AssociateView(typeof(BiographyOrderReportsComponentViewExtensionPoint))]
	public class BiographyOrderReportsComponent : ApplicationComponent
	{
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

		class ReportPreviewComponent : DHtmlComponent
		{
			// Internal data contract used for jscript deserialization
			[DataContract]
			public class ReportContext : DataContractBase
			{
				public ReportContext(EntityRef reportRef)
				{
					this.ReportRef = reportRef;
				}

				[DataMember]
				public EntityRef ReportRef;
			}

			private ReportContext _context;

			public ReportPreviewComponent(EntityRef reportRef)
			{
				_context = reportRef != null ? new ReportContext(reportRef) : null;
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
				return _context;
			}

			public ReportContext Context
			{
				get { return _context; }
				set
				{
					_context = value;
					Refresh();
				}
			}
		}

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
							return item.ProcedureType.Name;
						}).ToArray());

				s += " : " + _reportListItems[0].ReportStatus.Value;

				return s;
			}
		}

		private ReportsContext _context;

		private List<CommonReportListItem> _reports;
		private CommonReportListItem _selectedReport;

		private ReportPreviewComponent _reportPreviewComponent;
		private ChildComponentHost _reportPreviewComponentHost;

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
			RefreshOrderReports();

			_reportPreviewComponent = new ReportPreviewComponent(null);
			_reportPreviewComponentHost = new ChildComponentHost(this.Host, _reportPreviewComponent);
			_reportPreviewComponentHost.StartComponent();

			base.Start();
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
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
			get { return null; }
			set
			{
				if(!Equals(_selectedReport, value))
				{
					_selectedReport = value;
					ReportSelectionChanged();
				}
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
				RefreshOrderReports();
			}
		}

		private void RefreshOrderReports()
		{
			if (_context == null)
				return;

			Platform.GetService<IBrowsePatientDataService>(
				delegate(IBrowsePatientDataService service)
				{
					GetDataRequest request = new GetDataRequest();
					request.ListReportsRequest = new ListReportsRequest(_context.PatientRef, _context.OrderRef);

					GetDataResponse response = service.GetData(request);

					List<CommonReportListItem> reports = new List<CommonReportListItem>();

					CollectionUtils.ForEach<ReportListItem>(
						response.ListReportsResponse.Reports,
						delegate(ReportListItem item)
						{
							CommonReportListItem existingItem = CollectionUtils.SelectFirst(
								reports,
								delegate (CommonReportListItem crli)
								{
									return Equals(crli.ReportRef, item.ReportRef);
								});

							if(existingItem != null)
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
					ReportSelectionChanged();

					NotifyAllPropertiesChanged();
				});
		}

		private void ReportSelectionChanged()
		{
			_reportPreviewComponent.Context = _selectedReport != null ? new ReportPreviewComponent.ReportContext(_selectedReport.ReportRef) : null;
		}
	}
}