using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    public interface IOrderEntryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //PatientProfile LoadPatientProfile(EntityRef profileRef);
        LoadPatientProfileResponse LoadPatientProfile(LoadPatientProfileRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //IList<Visit> ListActiveVisits(EntityRef patientRef);
        ListActiveVisitsResponse ListActiveVisits(ListActiveVisitsRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //IList<DiagnosticService> ListDiagnosticServiceChoices();
        //IList<Facility> ListOrderingFacilityChoices();
        //IList<Practitioner> ListOrderingPhysicianChoices();
        GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //DiagnosticService LoadDiagnosticServiceBreakdown(EntityRef diagnosticServiceRef);
        LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //void PlaceOrder( Patient patient, Visit visit, DiagnosticService diagnosticService, OrderPriority priority,
        //    Practitioner orderingPhysician, Facility orderingFacility, DateTime schedulingRequestTime);
        PlaceOrderResponse PlaceOrder(PlaceOrderRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //IList<WorklistQueryResult> GetOrdersWorklist(ModalityProcedureStepSearchCriteria criteria);
        GetOrdersWorkListResponse GetOrdersWorkList(GetOrdersWorkListRequest request);
    }
}
