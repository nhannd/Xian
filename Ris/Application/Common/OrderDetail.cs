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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderDetail : DataContractBase
    {
        public OrderDetail()
        {
            this.Procedures = new List<ProcedureDetail>();
        }

        [DataMember] 
        public EntityRef OrderRef;

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public VisitDetail Visit;

        [DataMember]
        public string PlacerNumber;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public DiagnosticServiceSummary DiagnosticService;

        [DataMember]
        public DateTime? EnteredTime;

		[DataMember]
		public StaffSummary EnteredBy;

		[DataMember]
		public string EnteredComment;

		[DataMember]
        public DateTime? SchedulingRequestTime;

        [DataMember]
        public ExternalPractitionerSummary OrderingPractitioner;

        [DataMember]
        public FacilitySummary OrderingFacility;

        [DataMember]
        public string ReasonForStudy;

        [DataMember]
        public EnumValueInfo OrderPriority;

        [DataMember]
        public EnumValueInfo CancelReason;

		[DataMember]
		public StaffSummary CancelledBy;

		[DataMember]
		public string CancelComment;

		[DataMember]
        public List<ProcedureDetail> Procedures;

        [DataMember]
        public List<OrderNoteSummary> Notes;

		[DataMember]
		public List<AttachmentSummary> Attachments;

		[DataMember]
		public List<ResultRecipientDetail> ResultRecipients;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;
	}
}
