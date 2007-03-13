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
        public EntityRef MPSRef;

        [DataMember]
        public string MRNAssigningAuthority;

        [DataMember]
        public string MRNID;

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
