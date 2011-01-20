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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReportingWorklistItemSummary : WorklistItemSummaryBase
    {
        public ReportingWorklistItemSummary(
            EntityRef procedureStepRef,
            EntityRef procedureRef,
            EntityRef orderRef,
            EntityRef patientRef,
            EntityRef profileRef,
            EntityRef reportRef,
            CompositeIdentifierDetail mrn,
            PersonNameDetail name,
            string accessionNumber,
            EnumValueInfo orderPriority,
            EnumValueInfo patientClass,
            string diagnosticServiceName,
            string procedureName,
            bool procedurePortable,
            bool hasErrors,
            EnumValueInfo procedureLaterality,
            string procedureStepName,
            DateTime? time,
            EnumValueInfo activityStatus,
            int reportPartIndex)
            : base(
                procedureStepRef,
                procedureRef,
                orderRef,
                patientRef,
                profileRef,
                mrn,
                name,
                accessionNumber,
                orderPriority,
                patientClass,
                diagnosticServiceName,
                procedureName,
                procedurePortable,
                procedureLaterality,
                procedureStepName,
                time
            )
        {
            this.ReportRef = reportRef;
            this.ActivityStatus = activityStatus;
            this.ReportPartIndex = reportPartIndex;
            this.HasErrors = hasErrors;
        }

        [DataMember]
        public EntityRef ReportRef;

        [DataMember]
        public EnumValueInfo ActivityStatus;

        [DataMember]
        public int ReportPartIndex;

        [DataMember]
        public bool HasErrors;

        /// <summary>
        /// Gets a value indicating if this worklist item refers to an addendum.
        /// </summary>
        public bool IsAddendumStep
        {
            get { return this.ReportPartIndex > 0; }
        }
    }
}
