using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class DiagnosticServiceTreeItem : DataContractBase
    {
        public DiagnosticServiceTreeItem(EntityRef nodeRef, string description, DiagnosticServiceSummary diagnosticService)
        {
            this.NodeRef = nodeRef;
            this.Description = description;
            this.DiagnosticService = diagnosticService;
        }

        [DataMember]
        public EntityRef NodeRef;

        [DataMember]
        public string Description;

        [DataMember]
        public DiagnosticServiceSummary DiagnosticService;
    }
}
