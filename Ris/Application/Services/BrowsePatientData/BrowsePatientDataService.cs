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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alerts;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Application.Services.BrowsePatientData
{
	[ServiceImplementsContract(typeof(IBrowsePatientDataService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class BrowsePatientDataService : ApplicationServiceBase, IBrowsePatientDataService
	{
		#region IBrowsePatientDataService Members

		[ReadOperation]
		public GetDataResponse GetData(GetDataRequest request)
		{
			GetDataResponse response = new GetDataResponse();

			if (request.ListPatientProfilesRequest != null)
				response.ListPatientProfilesResponse = ListPatientProfiles(request.ListPatientProfilesRequest);

			if (request.GetPatientProfileDetailRequest != null)
				response.GetPatientProfileDetailResponse = GetPatientProfileDetail(request.GetPatientProfileDetailRequest);

			if (request.ListVisitsRequest != null)
				response.ListVisitsResponse = ListVisits(request.ListVisitsRequest);

			if (request.GetVisitDetailRequest != null)
				response.GetVisitDetailResponse = GetVisitDetail(request.GetVisitDetailRequest);

			if (request.ListOrdersRequest != null)
				response.ListOrdersResponse = ListOrders(request.ListOrdersRequest);

			if (request.GetOrderDetailRequest != null)
				response.GetOrderDetailResponse = GetOrderDetail(request.GetOrderDetailRequest);

			if (request.ListReportsRequest != null)
				response.ListReportsResponse = ListReports(request.ListReportsRequest);

			if (request.GetReportDetailRequest != null)
				response.GetReportDetailResponse = GetReportDetail(request.GetReportDetailRequest);

			return response;
		}

		#endregion


		private ListPatientProfilesResponse ListPatientProfiles(ListPatientProfilesRequest request)
		{
			Patient patient = PersistenceContext.Load<Patient>(request.PatientRef);

			PatientProfileAssembler assembler = new PatientProfileAssembler();
			return new ListPatientProfilesResponse(
				CollectionUtils.Map<PatientProfile, PatientProfileSummary>(
				patient.Profiles,
				delegate(PatientProfile profile)
				{
					return assembler.CreatePatientProfileSummary(profile, PersistenceContext);
				}));
		}

		private GetPatientProfileDetailResponse GetPatientProfileDetail(GetPatientProfileDetailRequest request)
		{
			PatientProfile profile = PersistenceContext.Load<PatientProfile>(request.PatientProfileRef);

			PatientProfileAssembler assembler = new PatientProfileAssembler();
			GetPatientProfileDetailResponse response = new GetPatientProfileDetailResponse();
			response.PatientProfile = assembler.CreatePatientProfileDetail(profile, this.PersistenceContext,
																				 request.IncludeAddresses,
																				 request.IncludeContactPersons,
																				 request.IncludeEmailAddresses,
																				 request.IncludeTelephoneNumbers,
																				 request.IncludeNotes,
																				 request.IncludeAttachments);
			if (request.IncludeAlerts)
			{
				List<AlertNotification> alerts = new List<AlertNotification>();
				alerts.AddRange(AlertHelper.Instance.Test(profile.Patient, this.PersistenceContext));
				alerts.AddRange(AlertHelper.Instance.Test(profile, this.PersistenceContext));

				AlertAssembler alertAssembler = new AlertAssembler();
				response.PatientAlerts =
					CollectionUtils.Map<AlertNotification, AlertNotificationDetail>(alerts,
					delegate(AlertNotification alert)
					{
						return alertAssembler.CreateAlertNotification(alert);
					});
			}

			return response;
		}

		private ListVisitsResponse ListVisits(ListVisitsRequest request)
		{
			BrowsePatientDataAssembler assembler = new BrowsePatientDataAssembler();

			Patient patient = PersistenceContext.Load<Patient>(request.PatientRef, EntityLoadFlags.Proxy);

			VisitSearchCriteria where = new VisitSearchCriteria();
			where.Patient.EqualTo(patient);

			IList<Visit> visits = PersistenceContext.GetBroker<IVisitBroker>().Find(where);
			return new ListVisitsResponse(
				CollectionUtils.Map<Visit, VisitListItem>(visits,
					delegate(Visit v)
					{
						return assembler.CreateVisitListItem(v, PersistenceContext);
					}));
		}

		private GetVisitDetailResponse GetVisitDetail(GetVisitDetailRequest request)
		{
			Visit visit = PersistenceContext.Load<Visit>(request.VisitRef, EntityLoadFlags.Proxy);

			VisitAssembler assembler = new VisitAssembler();
			VisitDetail detail = assembler.CreateVisitDetail(visit, PersistenceContext);

			return new GetVisitDetailResponse(detail);
		}

		private ListOrdersResponse ListOrders(ListOrdersRequest request)
		{
			BrowsePatientDataAssembler assembler = new BrowsePatientDataAssembler();

			Patient patient = PersistenceContext.Load<Patient>(request.PatientRef, EntityLoadFlags.Proxy);

			if (request.QueryDetailLevel == PatientOrdersQueryDetailLevel.Order)
			{
				return new ListOrdersResponse(
					CollectionUtils.Map<Order, OrderListItem>(
						PersistenceContext.GetBroker<IPatientHistoryBroker>().GetOrderHistory(patient),
						delegate(Order order)
						{
							return assembler.CreateOrderListItem(order, this.PersistenceContext);
						}));
			}
			else if (request.QueryDetailLevel == PatientOrdersQueryDetailLevel.Procedure)
			{
				return new ListOrdersResponse(
					CollectionUtils.Map<Procedure, OrderListItem>(
						PersistenceContext.GetBroker<IPatientHistoryBroker>().GetProcedureHistory(patient),
						delegate(Procedure rp)
						{
							return assembler.CreateOrderListItem(rp, this.PersistenceContext);
						}));
			}

			return new ListOrdersResponse(new List<OrderListItem>());
		}

		private GetOrderDetailResponse GetOrderDetail(GetOrderDetailRequest request)
		{
			Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);

			GetOrderDetailResponse response = new GetOrderDetailResponse();
			OrderAssembler assembler = new OrderAssembler();
			response.Order = assembler.CreateOrderDetail(order,
				this.PersistenceContext,
				request.IncludeVisit,
				request.IncludeProcedures,
				request.IncludeNotes,
				request.NoteCategoriesFilter,
				request.IncludeAttachments,
				request.IncludeResultRecipients,
				request.IncludeExtendedProperties);

			if (request.IncludeAlerts)
			{
				AlertAssembler alertAssembler = new AlertAssembler();
				response.OrderAlerts =
					CollectionUtils.Map<AlertNotification, AlertNotificationDetail>(
					AlertHelper.Instance.Test(order, this.PersistenceContext),
					delegate(AlertNotification alert)
					{
						return alertAssembler.CreateAlertNotification(alert);
					});
			}

			return response;
		}

		private ListReportsResponse ListReports(ListReportsRequest request)
		{
			IList<Report> reports = new List<Report>();
			if (request.OrderRef != null)
			{
				// list only reports for this order
				Order order = PersistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.Proxy);
				reports = PersistenceContext.GetBroker<IPatientHistoryBroker>().GetReportsForOrder(order);
			}
			else if (request.PatientRef != null)
			{
				Patient patient = PersistenceContext.Load<Patient>(request.PatientRef, EntityLoadFlags.Proxy);
				reports = PersistenceContext.GetBroker<IPatientHistoryBroker>().GetReportHistory(patient);
			}

			BrowsePatientDataAssembler assembler = new BrowsePatientDataAssembler();
			List<ReportListItem> reportListItems = new List<ReportListItem>();

			foreach (Report report in reports)
			{
				foreach (Procedure procedure in report.Procedures)
				{
					reportListItems.Add(assembler.CreateReportListItem(report, procedure, this.PersistenceContext));
				}
			}

			return new ListReportsResponse(reportListItems);
		}

		private GetReportDetailResponse GetReportDetail(GetReportDetailRequest request)
		{
			Report report = request.ReportRef != null 
				? PersistenceContext.Load<Report>(request.ReportRef)
				: PersistenceContext.Load<Procedure>(request.ProcedureRef).ActiveReport;

			return new GetReportDetailResponse(report == null ? null :
				new ReportAssembler().CreateReportDetail(report, request.IncludeCancelledParts, PersistenceContext));
		}
	}
}
