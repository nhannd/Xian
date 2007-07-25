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
        public ModalityWorklistItem(EntityRef procedureStepRef,
            string mrnAssigningAuthority,
            string mrnID,
            PersonNameDetail personNameDetail,
            string accessionNumber,
            string modalityProcedureStepName,
            string modalityName,
            EnumValueInfo priority)
        {
            this.ProcedureStepRef = procedureStepRef;
            this.MrnAssigningAuthority = mrnAssigningAuthority;
            this.MrnID = mrnID;
            this.PersonNameDetail = personNameDetail;
            this.AccessionNumber = accessionNumber;
            this.ModalityProcedureStepName = modalityProcedureStepName;
            this.ModalityName = modalityName;
            this.Priority = priority;
        }

        public ModalityWorklistItem()
        {
        }

        [DataMember]
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
        public string RequestedProcedureStepName;

        [DataMember]
        public string ModalityName;

        [DataMember]
        public EnumValueInfo Priority;

        public override bool Equals(object obj)
        {
            ModalityWorklistItem that = obj as ModalityWorklistItem;
            if (that != null)
                return this.ProcedureStepRef.Equals(that.ProcedureStepRef);

            return false;
        }

        public override int GetHashCode()
        {
            return this.ProcedureStepRef.GetHashCode();
        }
    }
}
