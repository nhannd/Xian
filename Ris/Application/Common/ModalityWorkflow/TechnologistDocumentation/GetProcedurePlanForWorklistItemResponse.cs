using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class GetProcedurePlanForWorklistItemResponse : DataContractBase
    {
        [DataMember]
        public List<RequestedProcedureDetail> RequestedProcedures;
    }
}