using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    public interface IOrderEntryService : IHealthcareServiceLayer
    {
        PatientProfile LoadPatientProfile(EntityRef<PatientProfile> profileRef);
        IList<Visit> ListActiveVisits(EntityRef<Patient> patientRef);
        IList<DiagnosticService> ListDiagnosticServiceChoices();
        IList<Facility> ListOrderingFacilityChoices();
        IList<Practitioner> ListOrderingPhysicianChoices();
        DiagnosticService LoadDiagnosticServiceBreakdown(EntityRef<DiagnosticService> diagnosticServiceRef);

        //string GenerateNewAccessionNumber();
        void PlaceOrder(
            Patient patient,
            Visit visit,
            DiagnosticService diagnosticService,
            OrderPriority priority,
            Practitioner orderingPhysician,
            Facility orderingFacility,
            DateTime schedulingRequestTime);

        IList<AcquisitionWorklistItem> GetOrdersWorklist(ScheduledProcedureStepSearchCriteria criteria);
    }
}
