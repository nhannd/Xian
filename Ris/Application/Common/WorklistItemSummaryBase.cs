using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class WorklistItemSummaryBase : DataContractBase
    {
        public WorklistItemSummaryBase(
            EntityRef procedureStepRef,
            EntityRef requestedProcedureRef,
            EntityRef orderRef,
            EntityRef patientRef,
            EntityRef profileRef,
            CompositeIdentifierDetail mrn,
            PersonNameDetail name,
            string accessionNumber,
            EnumValueInfo orderPriority,
            EnumValueInfo patientClass,
            string diagnosticServiceName,
            string requestedProcedureName,
            string procedureStepName,
            DateTime? scheduledStartTime)
        {
            this.ProcedureStepRef = procedureStepRef;
            this.RequestedProcedureRef = requestedProcedureRef;
            this.OrderRef = orderRef;
            this.PatientRef = patientRef;
            this.PatientProfileRef = profileRef;
            this.Mrn = mrn;
            this.PatientName = name;
            this.AccessionNumber = accessionNumber;
            this.OrderPriority = orderPriority;
            this.PatientClass = patientClass;
            this.DiagnosticServiceName = diagnosticServiceName;
            this.RequestedProcedureName = requestedProcedureName;
            this.ProcedureStepName = procedureStepName;
            this.ScheduledStartTime = scheduledStartTime;
        }

        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public EntityRef RequestedProcedureRef;

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public CompositeIdentifierDetail Mrn;

        [DataMember]
        public PersonNameDetail PatientName;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public EnumValueInfo OrderPriority;

        [DataMember]
        public EnumValueInfo PatientClass;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public string ProcedureStepName;

        [DataMember]
        public DateTime? ScheduledStartTime;

        public override bool Equals(object obj)
        {
            WorklistItemSummaryBase that = obj as WorklistItemSummaryBase;
            if (that != null)
                return Equals(this.ProcedureStepRef, that.ProcedureStepRef);

            return false;
        }

        public override int GetHashCode()
        {
            return this.ProcedureStepRef.GetHashCode();
        }
    }
}
