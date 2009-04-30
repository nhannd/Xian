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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class OrderRequisition : DataContractBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OrderRequisition()
        {
        }

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
        public List<OrderAttachmentSummary> Attachments;

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
