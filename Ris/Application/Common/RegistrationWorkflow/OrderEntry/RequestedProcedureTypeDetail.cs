using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class RequestedProcedureTypeDetail : DataContractBase
    {
        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

        [DataMember]
        public List<ModalityProcedureStepTypeDetail> ModalityProcedureStepTypes;
    }
}
