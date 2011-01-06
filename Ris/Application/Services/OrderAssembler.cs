#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
				orderDetail.ExtendedProperties = ExtendedPropertyUtils.Copy(order.ExtendedProperties);
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
