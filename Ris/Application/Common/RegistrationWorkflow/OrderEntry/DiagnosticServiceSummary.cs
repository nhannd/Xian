using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class DiagnosticServiceSummary : DataContractBase
    {
        public DiagnosticServiceSummary(EntityRef diagnosticServiceRef, string id, string name)
        {
            this.DiagnosticServiceRef = diagnosticServiceRef;
            this.Id = id;
            this.Name = name;
        }

        [DataMember]
        public EntityRef DiagnosticServiceRef;

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;
    }
}
