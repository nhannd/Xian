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
using ClearCanvas.Ris.Application.Common;
using System.Collections;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IOrderEntryService))]
    public class OrderEntryService : ApplicationServiceBase, IOrderEntryService
    {
        [ReadOperation]
        public ListActiveVisitsForPatientResponse ListActiveVisitsForPatient(ListActiveVisitsForPatientRequest request)
        {
            IPatientProfileBroker patientProfileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = patientProfileBroker.Load(request.PatientProfileRef, EntityLoadFlags.Proxy);

            VisitSearchCriteria criteria = new VisitSearchCriteria();
            criteria.Patient.EqualTo(profile.Patient);
            criteria.VisitStatus.NotEqualTo(VisitStatus.DC);

            VisitAssembler assembler = new VisitAssembler();
            return new ListActiveVisitsForPatientResponse(
                CollectionUtils.Map<Visit, VisitSummary, List<VisitSummary>>(
                    PersistenceContext.GetBroker<IVisitBroker>().Find(criteria),
                    delegate(Visit v)
                    {
                        return assembler.CreateVisitSummary(v, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request)
        {
            OrderEntryAssembler orderEntryAssembler = new OrderEntryAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();

            IList topLevelDiagnosticServiceTreeNodes = null;
            try 
	        {	        
		        DiagnosticServiceTreeNodeSearchCriteria rootNodeDiagnosticServiceTreeCriteria = new DiagnosticServiceTreeNodeSearchCriteria();
                rootNodeDiagnosticServiceTreeCriteria.Parent.IsNull();
                DiagnosticServiceTreeNode rootNode = PersistenceContext.GetBroker<IDiagnosticServiceTreeNodeBroker>().FindOne(rootNodeDiagnosticServiceTreeCriteria);
                topLevelDiagnosticServiceTreeNodes = rootNode.Children;
            }
	        catch (Exception)
	        {
                // no diagnostic service tree - just create an empty list
                topLevelDiagnosticServiceTreeNodes = new ArrayList();
	        }

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
                CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary, List<ExternalPractitionerSummary>>(
                    PersistenceContext.GetBroker<IExternalPractitionerBroker>().FindAll(),
                    delegate(ExternalPractitioner p)
                    {
                        return pracAssembler.CreateExternalPractitionerSummary(p, PersistenceContext);
                    }),
                EnumUtils.GetEnumValueList<OrderPriorityEnum>(PersistenceContext),
                CollectionUtils.Map<DiagnosticServiceTreeNode, DiagnosticServiceTreeItem, List<DiagnosticServiceTreeItem>>(
                    topLevelDiagnosticServiceTreeNodes,
                    delegate(DiagnosticServiceTreeNode n)
                    {
                        return orderEntryAssembler.CreateDiagnosticServiceTreeItem(n);
                    })

                );
        }

        [ReadOperation]
        public LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request)
        {
            IDiagnosticServiceBroker dsBroker = PersistenceContext.GetBroker<IDiagnosticServiceBroker>();

            DiagnosticService diagnosticService = dsBroker.Load(request.DiagnosticServiceRef);

            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new LoadDiagnosticServiceBreakdownResponse(assembler.CreateDiagnosticServiceDetail(diagnosticService));
        }


        [ReadOperation]
        public GetOrdersWorkListResponse GetOrdersWorkList(GetOrdersWorkListRequest request)
        {
            //TODO: remove this after adding the criteria into GetOrdersWorkListRequest
            //TODO: add validation to criteria that can throw a RequestValidationException
            //ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            
            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new GetOrdersWorkListResponse(
                CollectionUtils.Map<Order, OrderSummary, List<OrderSummary>>(
                    PersistenceContext.GetBroker<IOrderBroker>().FindAll(),
                    delegate(Order order)
                    {
                        return assembler.CreateOrderSummary(order, this.PersistenceContext);
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
            ExternalPractitioner orderingPhysician = PersistenceContext.GetBroker<IExternalPractitionerBroker>().Load(request.OrderingPhysician, EntityLoadFlags.Proxy);
            Facility orderingFacility = PersistenceContext.GetBroker<IFacilityBroker>().Load(request.OrderingFacility, EntityLoadFlags.Proxy);
            OrderPriority orderingPriority = EnumUtils.GetEnumValue<OrderPriority>(request.OrderPriority);
            DiagnosticService diagnosticService = PersistenceContext.GetBroker<IDiagnosticServiceBroker>().Load(request.DiagnosticService);

            IAccessionNumberBroker broker = PersistenceContext.GetBroker<IAccessionNumberBroker>();
            string accNum = broker.GetNextAccessionNumber();

            // TODO: add validation and throw RequestValidationException if necessary

            Order order = Order.NewOrder(
                    accNum, 
                    patient, 
                    visit, 
                    diagnosticService, 
                    request.SchedulingRequestTime, 
                    orderingPhysician, 
                    orderingFacility, 
                    EnumUtils.GetEnumValue<OrderPriority>(request.OrderPriority),
                    request.ScheduleOrder);

            PersistenceContext.Lock(order, DirtyState.New);

            // ensure the new order is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new PlaceOrderResponse(order.GetRef());
        }

        [ReadOperation]
        public ListOrdersForPatientResponse ListOrdersForPatient(ListOrdersForPatientRequest request)
        {
            OrderSearchCriteria criteria = new OrderSearchCriteria();

            PatientProfile profile = PersistenceContext.Load<PatientProfile>(request.PatientProfileRef);
            criteria.Patient.EqualTo(profile.Patient);
            
            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new ListOrdersForPatientResponse(
                CollectionUtils.Map<Order, OrderSummary, List<OrderSummary>>(
                    PersistenceContext.GetBroker<IOrderBroker>().Find(criteria),
                    delegate(Order order)
                    {
                        return assembler.CreateOrderSummary(order, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public LoadOrderDetailResponse LoadOrderDetail(LoadOrderDetailRequest request)
        {
            OrderEntryAssembler assembler = new OrderEntryAssembler();

            Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);

            return new LoadOrderDetailResponse(assembler.CreateOrderDetail(order, this.PersistenceContext));
        }

        [ReadOperation]
        public GetDiagnosticServiceSubTreeResponse GetDiagnosticServiceSubTree(GetDiagnosticServiceSubTreeRequest request)
        {
            DiagnosticServiceTreeNode node = PersistenceContext.Load<DiagnosticServiceTreeNode>(request.NodeRef, EntityLoadFlags.Proxy);

            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new GetDiagnosticServiceSubTreeResponse(
                CollectionUtils.Map<DiagnosticServiceTreeNode, DiagnosticServiceTreeItem, List<DiagnosticServiceTreeItem>>(
                    node.Children,
                    delegate(DiagnosticServiceTreeNode n)
                    {
                        return assembler.CreateDiagnosticServiceTreeItem(n);
                    }));
        }

    }
}
