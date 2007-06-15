using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReportingWorklistItem : DataContractBase
    {
        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public MrnDetail Mrn;

        [DataMember]
        public PersonNameDetail PersonNameDetail;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public string Priority;

        [DataMember]
        public EnumValueInfo ActivityStatus;

        [DataMember]
        public string StepType;
    }
}
