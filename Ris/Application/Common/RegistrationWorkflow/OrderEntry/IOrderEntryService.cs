using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    public interface IOrderEntryService
    {
        LoadPatientProfileResponse LoadPatientProfile(LoadPatientProfileRequest request);

        //PatientProfile LoadPatientProfile(EntityRef profileRef);

        ListActiveVisitsResponse ListActiveVisits(ListActiveVisitsRequest request);

        //IList<Visit> ListActiveVisits(EntityRef patientRef);

        GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request);

        //IList<DiagnosticService> ListDiagnosticServiceChoices();
        //IList<Facility> ListOrderingFacilityChoices();
        //IList<Practitioner> ListOrderingPhysicianChoices();

        LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request);
        
        //DiagnosticService LoadDiagnosticServiceBreakdown(EntityRef diagnosticServiceRef);

        ////string GenerateNewAccessionNumber();

        PlaceOrderResponse PlaceOrder(PlaceOrderRequest request);

        //void PlaceOrder(
        //    Patient patient,
        //    Visit visit,
        //    DiagnosticService diagnosticService,
        //    OrderPriority priority,
        //    Practitioner orderingPhysician,
        //    Facility orderingFacility,
        //    DateTime schedulingRequestTime);

        GetOrdersWorkListResponse GetOrdersWorkList(GetOrdersWorkListRequest request);
        
        //IList<WorklistQueryResult> GetOrdersWorklist(ModalityProcedureStepSearchCriteria criteria);
    }
}
