using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IModalityWorkflowService))]
    public class ModalityWorkflowService : WorkflowServiceBase, IModalityWorkflowService
    {
        public ModalityWorkflowService()
        {
            _worklistExtPoint = new ModalityWorklistExtensionPoint();
        }

        [ReadOperation]
        public SearchResponse Search(SearchRequest request)
        {
            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();

            IList<WorklistItem> result = PersistenceContext.GetBroker<IModalityWorklistBroker>().Search(
                request.SearchData.MrnID,
                request.SearchData.MrnAssigningAuthority,
                request.SearchData.HealthcardID,
                request.SearchData.FamilyName,
                request.SearchData.GivenName,
                request.SearchData.AccessionNumber,
                request.SearchData.ShowActiveOnly);

            return new SearchResponse(CollectionUtils.Map<WorklistItem, ModalityWorklistItem, List<ModalityWorklistItem>>(result,
                delegate(WorklistItem item)
                {
                    return assembler.CreateModalityWorklistItem(item, this.PersistenceContext);
                }));
        }

        [ReadOperation]
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            WorklistAssembler assembler = new WorklistAssembler();
            return new ListWorklistsResponse(
                CollectionUtils.Map<Worklist, WorklistSummary, List<WorklistSummary>>(
                    this.PersistenceContext.GetBroker<IWorklistBroker>().FindAllTechnologistWorklists(this.CurrentUser),
                    delegate(Worklist worklist)
                    {
                        return assembler.GetWorklistSummary(worklist);
                    }));
        }

        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();

            IList items = request.WorklistRef == null
                  ? GetWorklist(request.WorklistClassName)
                  : GetWorklist(request.WorklistRef);

            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, ModalityWorklistItem, List<ModalityWorklistItem>>(
                    items,
                    delegate(WorklistItem queryResult)
                    {
                        return assembler.CreateModalityWorklistItem(queryResult, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetWorklistCountResponse GetWorklistCount(GetWorklistCountRequest request)
        {
            int count = request.WorklistRef == null
                            ? GetWorklistCount(request.WorklistClassName)
                            : GetWorklistCount(request.WorklistRef);

            return new GetWorklistCountResponse(count);
        }

        [ReadOperation]
        public GetWorklistItemResponse GetWorklistItem(GetWorklistItemRequest request)
        {
            throw new NotImplementedException();
            //ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            //WorklistItem result = PersistenceContext.GetBroker<IModalityWorklistBroker>().GetWorklistItem(request.ProcedureStepRef, request.PatientProfileAuthority);
            //return new GetWorklistItemResponse(assembler.CreateModalityWorklistItem(result, this.PersistenceContext));
        }

        [ReadOperation]
        public LoadWorklistItemPreviewResponse LoadWorklistItemPreview(LoadWorklistItemPreviewRequest request)
        {
            //IModalityWorklistPreviewBroker broker = PersistenceContext.GetBroker<IModalityWorklistPreviewBroker>();
            //ModalityWorklistPreview preview = broker.GetPreview(request.ProcedureStepRef);
            //ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            //return new LoadWorklistItemPreviewResponse(assembler.CreateWorklistPreview(preview));

            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            ModalityProcedureStep mps = PersistenceContext.Load <ModalityProcedureStep> (request.ProcedureStepRef);
            return new LoadWorklistItemPreviewResponse(assembler.CreateWorklistPreview(mps, request.PatientProfileAuthority, PersistenceContext));
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(new WorklistItemKey(request.ProcedureStepRef)));
        }

        [UpdateOperation]
        [OperationEnablement("CanStartProcedure")]
        public StartProcedureResponse StartProcedure(StartProcedureRequest request)
        {
            ExecuteOperation(new StartModalityProcedureStepOperation(this.CurrentUserStaff), request.ModalityProcedureStepRef);
            return new StartProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteProcedure")]
        public CompleteProcedureResponse CompleteProcedure(CompleteProcedureRequest request)
        {
            // TODO determine procedureAborted logic
            ExecuteOperation(new CompleteModalityProcedureStepOperation(false), request.ModalityProcedureStepRef);
            return new CompleteProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelProcedure")]
        public CancelProcedureResponse CancelProcedure(CancelProcedureRequest request)
        {
            // TODO determine procedureAborted logic
            ExecuteOperation(new CancelModalityProcedureStepOperation(false), request.ModalityProcedureStepRef);
            return new CancelProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanReplaceOrder")]
        public ReplaceOrderResponse ReplaceOrder(ReplaceOrderRequest request)
        {
            PlaceOrderResponse placeOrderResponse = Platform.GetService<IOrderEntryService>().PlaceOrder(request.PlaceOrderRequest);

            // cancel order here    
            IOrderBroker broker = PersistenceContext.GetBroker<IOrderBroker>();
            OrderCancelReasonEnum reason = EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelOrderRequest.CancelReason, PersistenceContext);

            foreach (EntityRef orderRef in request.CancelOrderRequest.CancelledOrders)
            {
                Order order = broker.Load(orderRef, EntityLoadFlags.CheckVersion);
                if (order.Status == OrderStatus.SC)
                    order.Cancel(reason);
                else if (order.Status == OrderStatus.IP)
                    order.Discontinue(reason);
            }

            return new ReplaceOrderResponse(placeOrderResponse.OrderRef);
        }

        public bool CanStartProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new StartModalityProcedureStepOperation(this.CurrentUserStaff), itemKey);
        }

        public bool CanCompleteProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new CompleteModalityProcedureStepOperation(false), itemKey);
        }

        public bool CanCancelProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new CancelModalityProcedureStepOperation(false), itemKey);
        }

        public bool CanReplaceOrder(IWorklistItemKey itemKey)
        {
            IModalityProcedureStepBroker broker = PersistenceContext.GetBroker<IModalityProcedureStepBroker>();
            ModalityProcedureStep mps = broker.Load(((WorklistItemKey)itemKey).ModalityProcedureStep);
            Order order = mps.RequestedProcedure.Order;
            return order.Status == OrderStatus.SC || order.Status == OrderStatus.IP;
        }

        private void ExecuteOperation(ModalityOperation op, EntityRef modalityProcedureStepRef)
        {
            ModalityProcedureStep modalityProcedureStep = PersistenceContext.Load<ModalityProcedureStep>(modalityProcedureStepRef);
            op.Execute(modalityProcedureStep, new PersistentWorkflow(this.PersistenceContext));
        }

        private bool CanExecuteOperation(ModalityOperation op, IWorklistItemKey itemKey)
        {
            if (itemKey is WorklistItemKey)
            {
                ModalityProcedureStep modalityProcedureStep = PersistenceContext.Load<ModalityProcedureStep>(((WorklistItemKey)itemKey).ModalityProcedureStep);
                return op.CanExecute(modalityProcedureStep);
            }
            else
            {
                return false;
            }
        }
    }
}
