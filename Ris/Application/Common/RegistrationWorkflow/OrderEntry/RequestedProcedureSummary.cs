using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class RequestedProcedureSummary : DataContractBase
    {
        public RequestedProcedureSummary(EntityRef rpRef, EntityRef orderRef, string index, RequestedProcedureTypeDetail type, List<ModalityProcedureStepSummary> procedureSteps)
        {
            this.RequestedProcedureRef = rpRef;
            this.OrderRef = orderRef;
            this.Index = index;
            this.Type = type;
            this.ProcedureSteps = procedureSteps;
        }

        public RequestedProcedureSummary()
        {
            this.ProcedureSteps = new List<ModalityProcedureStepSummary>();
        }

        [DataMember]
        public EntityRef RequestedProcedureRef;

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public string Index;

        [DataMember]
        public RequestedProcedureTypeDetail Type;

        [DataMember]
        public List<ModalityProcedureStepSummary> ProcedureSteps;
    }
}
