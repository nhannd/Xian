#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class OrderRequisition : DataContractBase
	{
		/// <summary>
		/// Patient for which procedures are being ordered. Required for new orders. Ignored for order modification.
		/// </summary>
		[DataMember]
		public EntityRef Patient;

		/// <summary>
		/// Visit with which the order is associated. Required.
		/// </summary>
		[DataMember]
		public VisitSummary Visit;

		/// <summary>
		/// Diagnostic service to order. Required for new orders. Ignored for order modification.
		/// </summary>
		[DataMember]
		public DiagnosticServiceSummary DiagnosticService;

		/// <summary>
		/// Reason that the procedures are being ordered. Required.
		/// </summary>
		[DataMember]
		public string ReasonForStudy;

		/// <summary>
		/// Order priority. Required.
		/// </summary>
		[DataMember]
		public EnumValueInfo Priority;

		/// <summary>
		/// The set of procedures being requested. If not provided, the default set of procedures
		/// for the diagnostic service will be ordered.
		/// When modifying an order, existing procedures will be updated from procedures in this list,
		/// and any new procedures in the list will be added to the order.  Any procedure previously
		/// in the order that are not found in the list will be removed from the order.
		/// </summary>
		[DataMember]
		public List<ProcedureRequisition> Procedures;

		/// <summary>
		/// Facility that is placing the order. Required.
		/// </summary>
		[DataMember]
		public FacilitySummary OrderingFacility;

		/// <summary>
		/// Time that the procedures are requested to be scheduled for, if not actually being scheduled now. Optional.
		/// </summary>
		[DataMember]
		public DateTime? SchedulingRequestTime;

		/// <summary>
		/// Practitioner on behalf of whom the order is being placed. Required.
		/// </summary>
		[DataMember]
		public ExternalPractitionerSummary OrderingPractitioner;

		/// <summary>
		/// List of recipients to receive results of the order.
		/// </summary>
		[DataMember]
		public List<ResultRecipientDetail> ResultRecipients;

		/// <summary>
		/// A list of attachments for this order.  Optional.
		/// </summary>
		[DataMember]
		public List<AttachmentSummary> Attachments;

		/// <summary>
		/// A list of notes for this order.  Optional.
		/// </summary>
		[DataMember]
		public List<OrderNoteDetail> Notes;

		/// <summary>
		/// A dictionary of extended properties for this order.  Optional.
		/// </summary>
		[DataMember]
		public Dictionary<string, string> ExtendedProperties;

		/// <summary>
		/// A downtime accession number, if this requisition represents an order that was performed during downtime. Optional.
		/// If this field is populated, the order will use this accession number instead of generating a new accession number.
		/// </summary>
		[DataMember]
		public string DowntimeAccessionNumber;

		/// <summary>
		/// Gets a value indicating whether this requisition is for a downtime order.
		/// </summary>
		public bool IsDowntimeOrder
		{
			get { return !string.IsNullOrEmpty(DowntimeAccessionNumber); }
		}
	}
}
