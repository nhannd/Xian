using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class GetOrderEntryFormDataRequest : DataContractBase
    {
        public GetOrderEntryFormDataRequest(EntityRef diagnosticServiceRef)
        {
            this.DiagnosticServiceRef = diagnosticServiceRef;
        }

        [DataMember]
        public EntityRef DiagnosticServiceRef;
    }
}
