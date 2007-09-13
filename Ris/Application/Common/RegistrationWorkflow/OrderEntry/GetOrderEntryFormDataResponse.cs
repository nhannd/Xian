using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class GetOrderEntryFormDataResponse : DataContractBase
    {
        public GetOrderEntryFormDataResponse(
            List<DiagnosticServiceSummary> diagnosticServiceChoices,
            List<FacilitySummary> orderingFacilityChoices,
            List<ExternalPractitionerSummary> orderingPhysicianChoices,
            List<EnumValueInfo> orderPriorityChoices,
            List<EnumValueInfo> cancelReasonChoices,
            List<DiagnosticServiceTreeItem> topLevelDiagnosticServiceTree)
        {
            this.DiagnosticServiceChoices = diagnosticServiceChoices;
            this.OrderingFacilityChoices = orderingFacilityChoices;
            this.OrderingPhysicianChoices = orderingPhysicianChoices;
            this.OrderPriorityChoices = orderPriorityChoices;
            this.CancelReasonChoices = cancelReasonChoices;
            this.TopLevelDiagnosticServiceTree = topLevelDiagnosticServiceTree;
        }

        [DataMember]
        public List<DiagnosticServiceSummary> DiagnosticServiceChoices;

        [DataMember]
        public List<FacilitySummary> OrderingFacilityChoices;

        [DataMember]
        public List<ExternalPractitionerSummary> OrderingPhysicianChoices;

        [DataMember]
        public List<EnumValueInfo> OrderPriorityChoices;

        [DataMember]
        public List<EnumValueInfo> CancelReasonChoices;

        [DataMember]
        public List<DiagnosticServiceTreeItem> TopLevelDiagnosticServiceTree;
    }
}
