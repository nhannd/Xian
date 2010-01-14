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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services
{
	public class OrderAssembler
	{
		/// <summary>
		/// Creates fully verbose order detail document.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public OrderDetail CreateOrderDetail(Order order, IPersistenceContext context)
		{
			return CreateOrderDetail(order, new CreateOrderDetailOptions(true, true, true, null, true, true, true), context);
		}

		public class CreateOrderDetailOptions
		{
			public CreateOrderDetailOptions(bool includeVisit, bool includeProcedures, bool includeNotes, IList<string> noteCategoriesFilter, bool includeAttachments, bool includeResultRecipients, bool includeExtendedProperties)
			{
				IncludeVisit = includeVisit;
				IncludeProcedures = includeProcedures;
				IncludeNotes = includeNotes;
				NoteCategoriesFilter = noteCategoriesFilter;
				IncludeAttachments = includeAttachments;
				IncludeResultRecipients = includeResultRecipients;
				IncludeExtendedProperties = includeExtendedProperties;
			}

			public bool IncludeVisit { get; private set; }
			public bool IncludeProcedures { get; private set; }
			public bool IncludeNotes { get; private set; }
			public IList<string> NoteCategoriesFilter { get; private set; }
			public bool IncludeAttachments { get; private set; }
			public bool IncludeResultRecipients { get; private set; }
			public bool IncludeExtendedProperties { get; private set; }
		}

		/// <summary>
		/// Creates order detail document including only specified parts.
		/// </summary>
		public OrderDetail CreateOrderDetail(Order order, CreateOrderDetailOptions options, IPersistenceContext context)
		{
			var orderDetail = new OrderDetail();

			var practitionerAssembler = new ExternalPractitionerAssembler();
			var facilityAssembler = new FacilityAssembler();
			var dsAssembler = new DiagnosticServiceAssembler();
			var procedureAssembler = new ProcedureAssembler();
			var staffAssembler = new StaffAssembler();

			orderDetail.OrderRef = order.GetRef();
			orderDetail.PatientRef = order.Patient.GetRef();

			if (options.IncludeVisit)
			{
				var visitAssembler = new VisitAssembler();
				orderDetail.Visit = visitAssembler.CreateVisitDetail(order.Visit, context);
			}

			orderDetail.PlacerNumber = order.PlacerNumber;
			orderDetail.AccessionNumber = order.AccessionNumber;
			orderDetail.DiagnosticService = dsAssembler.CreateSummary(order.DiagnosticService);

			orderDetail.EnteredTime = order.EnteredTime;
			orderDetail.EnteredBy = order.EnteredBy == null ? null :
				staffAssembler.CreateStaffSummary(order.EnteredBy, context);
			orderDetail.EnteredComment = order.EnteredComment;

			orderDetail.SchedulingRequestTime = order.SchedulingRequestTime;
			orderDetail.OrderingPractitioner = practitionerAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context);
			orderDetail.OrderingFacility = facilityAssembler.CreateFacilitySummary(order.OrderingFacility);
			orderDetail.ReasonForStudy = order.ReasonForStudy;
			orderDetail.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);

			if (order.CancelInfo != null)
			{
				orderDetail.CancelReason = order.CancelInfo.Reason == null ? null : EnumUtils.GetEnumValueInfo(order.CancelInfo.Reason);
				orderDetail.CancelledBy = order.CancelInfo.CancelledBy == null ? null :
					staffAssembler.CreateStaffSummary(order.CancelInfo.CancelledBy, context);
				orderDetail.CancelComment = order.CancelInfo.Comment;
			}

			if (options.IncludeProcedures)
			{
				orderDetail.Procedures = CollectionUtils.Map<Procedure, ProcedureDetail>(
					order.Procedures,
					procedure => procedureAssembler.CreateProcedureDetail(procedure, context));
			}

			if (options.IncludeNotes)
			{
				var orderNoteAssembler = new OrderNoteAssembler();
				var orderNotes = new List<OrderNote>(OrderNote.GetNotesForOrder(order));

				// apply category filter, if provided
				if (options.NoteCategoriesFilter != null && options.NoteCategoriesFilter.Count > 0)
				{
					orderNotes = CollectionUtils.Select(
						orderNotes,
						n => options.NoteCategoriesFilter.Contains(n.Category));
				}

				// sort notes by post-time (guaranteed non-null because only "posted" notes are in this collection)
				orderNotes.Sort((x, y) => x.PostTime.Value.CompareTo(y.PostTime.Value));

				// Put most recent notes first
				orderNotes.Reverse();

				orderDetail.Notes = CollectionUtils.Map<OrderNote, OrderNoteSummary>(
					orderNotes,
					note => orderNoteAssembler.CreateOrderNoteSummary(note, context));
			}

			if (options.IncludeAttachments)
			{
				var orderAttachmentAssembler = new OrderAttachmentAssembler();
				var attachments = new List<OrderAttachment>(order.Attachments);

				orderDetail.Attachments = CollectionUtils.Map<OrderAttachment, OrderAttachmentSummary>(
					attachments,
					a => orderAttachmentAssembler.CreateOrderAttachmentSummary(a, context));
			}

			if (options.IncludeResultRecipients)
			{
				var resultRecipientAssembler = new ResultRecipientAssembler();
				var resultRecipients = new List<ResultRecipient>(order.ResultRecipients);

				orderDetail.ResultRecipients = CollectionUtils.Map<ResultRecipient, ResultRecipientDetail>(
					resultRecipients,
					r => resultRecipientAssembler.CreateResultRecipientDetail(r, context));
			}

			if (options.IncludeExtendedProperties)
			{
				orderDetail.ExtendedProperties = new Dictionary<string, string>(order.ExtendedProperties);
			}

			return orderDetail;
		}

		public OrderSummary CreateOrderSummary(Order order, IPersistenceContext context)
		{
			var practitionerAssembler = new ExternalPractitionerAssembler();
			return new OrderSummary
				{
					OrderRef = order.GetRef(),
					AccessionNumber = order.AccessionNumber,
					DiagnosticServiceName = order.DiagnosticService.Name,
					EnteredTime = order.EnteredTime,
					SchedulingRequestTime = order.SchedulingRequestTime,
					OrderingPractitioner = practitionerAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context),
					OrderingFacility = order.OrderingFacility.Name,
					ReasonForStudy = order.ReasonForStudy,
					OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context),
					OrderStatus = EnumUtils.GetEnumValueInfo(order.Status, context)
				};
		}
	}
}
