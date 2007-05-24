using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class GetDiagnosticServiceSubTreeRequest : DataContractBase
    {
        public GetDiagnosticServiceSubTreeRequest(EntityRef nodeRef)
        {
            this.NodeRef = nodeRef;
        }

        [DataMember]
        public EntityRef NodeRef;
    }
}
