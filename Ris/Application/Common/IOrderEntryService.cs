using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    public interface IOrderEntryService
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
