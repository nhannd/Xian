using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Healthcare.Workflow;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IModalityWorkflowService))]
    public class ModalityWorkflowService : WorkflowServiceBase, IModalityWorkflowService
    {
        public ModalityWorkflowService()
        {
            _worklistExtPoint = new ClearCanvas.Healthcare.Workflow.Modality.WorklistExtensionPoint();
        }

        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();

            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, ModalityWorklistItem, List<ModalityWorklistItem>>(
                    GetWorklist(request.WorklistClassName),
                    delegate(WorklistItem queryResult)
                    {
                        return assembler.CreateModalityWorklistItem(queryResult, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetWorklistCountResponse GetWorklistCount(GetWorklistCountRequest request)
        {
            return new GetWorklistCountResponse(
                GetWorklistCount(request.WorklistClassName));
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
            IModalityProcedureStepBroker spsBroker = PersistenceContext.GetBroker<IModalityProcedureStepBroker>();
            IRequestedProcedureBroker rpBroker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = PersistenceContext.GetBroker<IOrderBroker>();
            IPatientBroker patientBroker = PersistenceContext.GetBroker<IPatientBroker>();

            ModalityProcedureStep sps = spsBroker.Load(request.ProcedureStepRef);
            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            return new LoadWorklistItemPreviewResponse(assembler.CreateWorklistPreview(sps, request.PatientProfileAuthority));
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(new WorklistItemKey(request.WorklistItem.ProcedureStepRef)));
        }

        [UpdateOperation]
        [OperationEnablement("CanStartProcedure")]
        public StartProcedureResponse StartProcedure(StartProcedureRequest request)
        {
            ModalityProcedureStep modalityProcedureStep = (ModalityProcedureStep)PersistenceContext.Load(request.ModalityProcedureStepRef);
            StartModalityProcedureStepOperation op = new StartModalityProcedureStepOperation();
            op.Execute(modalityProcedureStep, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
            return new StartProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanSuspendProcedure")]
        public SuspendProcedureResponse SuspendProcedure(SuspendProcedureRequest request)
        {
            ModalityProcedureStep modalityProcedureStep = (ModalityProcedureStep)PersistenceContext.Load(request.ModalityProcedureStepRef);
            SuspendModalityProcedureStepOperation op = new SuspendModalityProcedureStepOperation();
            op.Execute(modalityProcedureStep, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
            return new SuspendProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteProcedure")]
        public CompleteProcedureResponse CompleteProcedure(CompleteProcedureRequest request)
        {
            ModalityProcedureStep modalityProcedureStep = (ModalityProcedureStep)PersistenceContext.Load(request.ModalityProcedureStepRef);
            CompleteModalityProcedureStepOperation op = new CompleteModalityProcedureStepOperation();
            op.Execute(modalityProcedureStep, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
            return new CompleteProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelProcedure")]
        public CancelProcedureResponse CancelProcedure(CancelProcedureRequest request)
        {
            ModalityProcedureStep modalityProcedureStep = (ModalityProcedureStep)PersistenceContext.Load(request.ModalityProcedureStepRef);
            CancelModalityProcedureStepOperation op = new CancelModalityProcedureStepOperation();
            op.Execute(modalityProcedureStep, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
            return new CancelProcedureResponse();
        }

        public bool CanStartProcedure(IWorklistItemKey itemKey)
        {
            if (itemKey is WorklistItemKey)
            {
                ModalityProcedureStep modalityProcedureStep = (ModalityProcedureStep)PersistenceContext.Load(((WorklistItemKey)itemKey).ModalityProcedureStep);
                StartModalityProcedureStepOperation op = new StartModalityProcedureStepOperation();
                return op.CanExecute(modalityProcedureStep);
            }
            else
            {
                return false;
            }
        }

        public bool CanSuspendProcedure(IWorklistItemKey itemKey)
        {
            if (itemKey is WorklistItemKey)
            {
                ModalityProcedureStep modalityProcedureStep = (ModalityProcedureStep)PersistenceContext.Load(((WorklistItemKey)itemKey).ModalityProcedureStep);
                SuspendModalityProcedureStepOperation op = new SuspendModalityProcedureStepOperation();
                return op.CanExecute(modalityProcedureStep);
            }
            else
            {
                return false;
            }
        }

        public bool CanCompleteProcedure(IWorklistItemKey itemKey)
        {
            if (itemKey is WorklistItemKey)
            {
                ModalityProcedureStep modalityProcedureStep = (ModalityProcedureStep)PersistenceContext.Load(((WorklistItemKey)itemKey).ModalityProcedureStep);
                CompleteModalityProcedureStepOperation op = new CompleteModalityProcedureStepOperation();
                return op.CanExecute(modalityProcedureStep);
            }
            else
            {
                return false;
            }
        }

        public bool CanCancelProcedure(IWorklistItemKey itemKey)
        {
            if (itemKey is WorklistItemKey)
            {
                ModalityProcedureStep modalityProcedureStep = (ModalityProcedureStep)PersistenceContext.Load(((WorklistItemKey)itemKey).ModalityProcedureStep);
                CancelModalityProcedureStepOperation op = new CancelModalityProcedureStepOperation();
                return op.CanExecute(modalityProcedureStep);
            }
            else
            {
                return false;
            }
        }

    }
}
