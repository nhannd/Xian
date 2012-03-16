#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ReportPartDetail : DataContractBase
    {
        public ReportPartDetail(
            EntityRef reportPartRef,
            int index,
            bool isAddendum,
            EnumValueInfo status,
            DateTime creationTime,
            DateTime? preliminaryTime,
            DateTime? completedTime,
            DateTime? cancelledTime,
            StaffSummary supervisor,
            StaffSummary interpretedBy,
            StaffSummary transcribedBy,
            StaffSummary transcriptionSupervisor,
            StaffSummary verifiedBy,
            EnumValueInfo transcriptionRejectReason,
            Dictionary<string, string> extendedProperties)
        {
            this.ReportPartRef = reportPartRef;
            this.Index = index;
            this.IsAddendum = isAddendum;
            this.Status = status;
            this.CreationTime = creationTime;
            this.PreliminaryTime = preliminaryTime;
            this.CompletedTime = completedTime;
            this.CancelledTime = cancelledTime;
            this.Supervisor = supervisor;
            this.InterpretedBy = interpretedBy;
            this.TranscribedBy = transcribedBy;
            this.TranscriptionSupervisor = transcriptionSupervisor;
            this.VerifiedBy = verifiedBy;
            this.TranscriptionRejectReason = transcriptionRejectReason;
            this.ExtendedProperties = extendedProperties;
        }

        public static string ReportContentKey = "ReportContent";

        [DataMember]
        public EntityRef ReportPartRef;

        [DataMember]
        public int Index;

        [DataMember]
        public bool IsAddendum;

        [DataMember]
        public EnumValueInfo Status;

        [DataMember]
        public DateTime CreationTime;

        [DataMember]
        public DateTime? PreliminaryTime;

        [DataMember]
        public DateTime? CompletedTime;

        [DataMember]
        public DateTime? CancelledTime;

        [DataMember]
        public StaffSummary Supervisor;

        [DataMember]
        public StaffSummary InterpretedBy;

        [DataMember]
        public StaffSummary TranscribedBy;

        [DataMember]
        public StaffSummary TranscriptionSupervisor;

        [DataMember]
        public StaffSummary VerifiedBy;

        [DataMember]
        public EnumValueInfo TranscriptionRejectReason;

        [DataMember]
        public Dictionary<string, string> ExtendedProperties;

        public string Content
        {
            get
            {
                if (ExtendedProperties == null || !ExtendedProperties.ContainsKey(ReportContentKey))
                    return null;

                return ExtendedProperties[ReportContentKey];
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (ExtendedProperties != null && ExtendedProperties.ContainsKey(ReportContentKey))
                    {
                        ExtendedProperties.Remove(ReportContentKey);
                    }
                }
                else
                {
                    if (ExtendedProperties == null)
                        ExtendedProperties = new Dictionary<string, string>();

                    ExtendedProperties[ReportContentKey] = value;
                }
            }
        }
    }
}