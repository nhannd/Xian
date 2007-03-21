using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class DiagnosticServiceDetail : DataContractBase
    {
        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

        [DataMember]
        public List<RequestedProcedureTypeDetail> RequestedProcedureTypes;
    }
}
