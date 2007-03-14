using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ModalityWorklistItem : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public string MrnAssigningAuthority;

        [DataMember]
        public string MrnID;

        [DataMember]
        public PersonNameDetail PersonNameDetail;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string ModalityProcedureStepName;

        [DataMember]
        public string ModalityName;

        [DataMember]
        public string Priority;
    }
}
