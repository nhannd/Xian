using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Services
{
    public interface IOrderEntryService : IHealthcareServiceLayer
    {
        PatientProfile LoadPatientProfile(EntityRef profileRef);
        IList<Visit> ListActiveVisits(EntityRef patientRef);
        IList<DiagnosticService> ListDiagnosticServiceChoices();
        IList<Facility> ListOrderingFacilityChoices();
        IList<Practitioner> ListOrderingPhysicianChoices();
        DiagnosticService LoadDiagnosticServiceBreakdown(EntityRef diagnosticServiceRef);

        //string GenerateNewAccessionNumber();
        void PlaceOrder(
            Patient patient,
            Visit visit,
            DiagnosticService diagnosticService,
            OrderPriority priority,
            Practitioner orderingPhysician,
            Facility orderingFacility,
            DateTime schedulingRequestTime);

        IList<WorklistQueryResult> GetOrdersWorklist(ModalityProcedureStepSearchCriteria criteria);
    }
}
