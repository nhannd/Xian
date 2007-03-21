using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Services.Admin;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class OrderEntryService : ApplicationServiceBase, IOrderEntryService
    {
        [ReadOperation]
        public ListActiveVisitsForPatientResponse ListActiveVisitsForPatient(ListActiveVisitsForPatientRequest request)
        {
            IPatientProfileBroker patientProfileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = patientProfileBroker.Load(request.PatientProfileRef, EntityLoadFlags.Proxy);
            patientProfileBroker.LoadPatientForPatientProfile(profile);

            // ensure that the profiles collection is loaded
            Patient patient = PersistenceContext.GetBroker<IPatientBroker>().Load(request.PatientProfileRef, EntityLoadFlags.Proxy);

            VisitSearchCriteria criteria = new VisitSearchCriteria();
            criteria.Patient.EqualTo(patient);
            criteria.VisitStatus.NotEqualTo(VisitStatus.Discharged);

            VisitAssembler assembler = new VisitAssembler();
            return new ListActiveVisitsForPatientResponse(
                CollectionUtils.Map<Visit, VisitDetail, List<VisitSummary>>(
                    PersistenceContext.GetBroker<IVisitBroker>().Find(criteria),
                    delegate(Visit v)
                    {
                        return assembler.CreateVisitSummary(v);
                    }));
        }

        [ReadOperation]
        public GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request)
        {
            OrderEntryAssembler orderEntryAssembler = new OrderEntryAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            PractitionerAssembler practitionerAssembler = new PractitionerAssembler();

            // TODO: figure out how to determine which physicians are "ordering" physicians
            return new GetOrderEntryFormDataResponse(
                CollectionUtils.Map<DiagnosticService, DiagnosticServiceSummary, List<DiagnosticServiceSummary>>(
                    PersistenceContext.GetBroker<IDiagnosticServiceBroker>().FindAll(),
                    delegate(DiagnosticService ds)
                    {
                        return orderEntryAssembler.CreateDiagnosticServiceSummary(ds);
                    }), 
                CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
                    PersistenceContext.GetBroker<IFacilityBroker>().FindAll(),
                    delegate(Facility f)
                    {
                        return facilityAssembler.CreateFacilitySummary(f);
                    }), 
                CollectionUtils.Map<Practitioner, PractitionerSummary, List<PractitionerSummary>>(
                    PersistenceContext.GetBroker<IPractitionerBroker>().FindAll(),
                    delegate(Practitioner p)
                    {
                        return practitionerAssembler.CreatePractitionerSummary(p);
                    })
                );
        }

        [ReadOperation]
        public LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request)
        {
            IDiagnosticServiceBroker dsBroker = PersistenceContext.GetBroker<IDiagnosticServiceBroker>();
            IRequestedProcedureTypeBroker rptBroker = PersistenceContext.GetBroker<IRequestedProcedureTypeBroker>();

            DiagnosticService diagnosticService = dsBroker.Load(request.DiagnosticServiceRef);
            foreach (RequestedProcedureType rpt in diagnosticService.RequestedProcedureTypes)
            {
                rptBroker.LoadModalityProcedureStepTypesForRequestedProcedureType(rpt);
            }

            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new LoadDiagnosticServiceBreakdownResponse(assembler.CreateDiagnosticServiceDetail(diagnosticService));
        }


        [ReadOperation]
        public GetOrdersWorkListResponse GetOrdersWorkList(GetOrdersWorkListRequest request)
        {
            //TODO: remove this after adding the criteria into GetOrdersWorkListRequest
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();

            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new GetOrdersWorkListResponse(
                CollectionUtils.Map<WorklistQueryResult, OrderSummary, List<OrderSummary>>(
                    PersistenceContext.GetBroker<IModalityWorklistBroker>().GetWorklist(criteria, request.PatientProfileAuthority),
                    delegate(WorklistQueryResult result)
                    {
                        return assembler.CreateOrderSummary(result, this.PersistenceContext);
                    }));
        }

/*
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public string GenerateNewAccessionNumber()
        {
            // note that we have declared this method as requiring a new persistence context
            // it is safer to generate accession numbers in a separate transaction always
            // if an accession number were generated in a shared transaction, and that transaction was rolled-back,
            // the accession sequence would rollback as well, leaving the application with the possibility of a
            // duplicate accession number
            IAccessionNumberBroker broker = this.CurrentContext.GetBroker<IAccessionNumberBroker>();
            return broker.GetNextAccessionNumber();
        }
*/
        [UpdateOperation]
        public PlaceOrderResponse PlaceOrder(PlaceOrderRequest request)
        {
            Patient patient = PersistenceContext.GetBroker<IPatientBroker>().Load(request.Patient, EntityLoadFlags.Proxy);
            Visit visit = PersistenceContext.GetBroker<IVisitBroker>().Load(request.Visit, EntityLoadFlags.Proxy);
            Practitioner orderingPhysician = PersistenceContext.GetBroker<IPractitionerBroker>().Load(request.OrderingPhysician, EntityLoadFlags.Proxy);
            Facility orderingFacility = PersistenceContext.GetBroker<IFacilityBroker>().Load(request.OrderingFacility, EntityLoadFlags.Proxy);
            OrderPriority orderingPriority = (OrderPriority)Enum.Parse(typeof(OrderPriority), request.OrderPriority.Code);

            DiagnosticService diagnosticService = PersistenceContext.GetBroker<IDiagnosticServiceBroker>().Load(request.DiagnosticService);
            IRequestedProcedureTypeBroker rptBroker = PersistenceContext.GetBroker<IRequestedProcedureTypeBroker>();
            foreach (RequestedProcedureType rpt in diagnosticService.RequestedProcedureTypes)
            {
                rptBroker.LoadModalityProcedureStepTypesForRequestedProcedureType(rpt);
            }

            this.CurrentContext.Lock(patient);
            this.CurrentContext.Lock(visit);
            this.CurrentContext.Lock(diagnosticService);
            this.CurrentContext.Lock(orderingPhysician);
            this.CurrentContext.Lock(orderingFacility);

            IAccessionNumberBroker broker = PersistenceContext.GetBroker<IAccessionNumberBroker>();
            string accNum = broker.GetNextAccessionNumber();

            Order order = Order.NewOrder(
                accNum, patient, visit, diagnosticService, schedulingRequestTime, orderingPhysician, orderingFacility, priority);

            PersistenceContext.Lock(order, DirtyState.New);

            // ensure the new order is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new PlaceOrderResponse(order.GetRef());
        }
    }
}
