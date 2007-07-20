using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

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
            ExecuteOperation(new StartModalityProcedureStepOperation(), request.ModalityProcedureStepRef);
            return new StartProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanSuspendProcedure")]
        public SuspendProcedureResponse SuspendProcedure(SuspendProcedureRequest request)
        {
            ExecuteOperation(new SuspendModalityProcedureStepOperation(), request.ModalityProcedureStepRef);
            return new SuspendProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanResumeProcedure")]
        public ResumeProcedureResponse ResumeProcedure(ResumeProcedureRequest request)
        {
            ExecuteOperation(new ResumeModalityProcedureStepOperation(), request.ModalityProcedureStepRef);
            return new ResumeProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteProcedure")]
        public CompleteProcedureResponse CompleteProcedure(CompleteProcedureRequest request)
        {
            ExecuteOperation(new CompleteModalityProcedureStepOperation(), request.ModalityProcedureStepRef);
            return new CompleteProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelProcedure")]
        public CancelProcedureResponse CancelProcedure(CancelProcedureRequest request)
        {
            ExecuteOperation(new CancelModalityProcedureStepOperation(), request.ModalityProcedureStepRef);
            return new CancelProcedureResponse();
        }

        public bool CanStartProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new StartModalityProcedureStepOperation(), itemKey);
        }

        public bool CanSuspendProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new SuspendModalityProcedureStepOperation(), itemKey);
        }

        public bool CanResumeProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new ResumeModalityProcedureStepOperation(), itemKey);
        }

        public bool CanCompleteProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new CompleteModalityProcedureStepOperation(), itemKey);
        }

        public bool CanCancelProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new CancelModalityProcedureStepOperation(), itemKey);
        }

        private void ExecuteOperation(ModalityOperation op, EntityRef modalityProcedureStepRef)
        {
            ModalityProcedureStep modalityProcedureStep = PersistenceContext.Load<ModalityProcedureStep>(modalityProcedureStepRef);
            op.Execute(modalityProcedureStep, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
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
