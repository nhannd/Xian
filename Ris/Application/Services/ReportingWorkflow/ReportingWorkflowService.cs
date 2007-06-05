using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    [ServiceImplementsContract(typeof(IReportingWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ReportingWorkflowService : WorkflowServiceBase, IReportingWorkflowService
    {
        public ReportingWorkflowService()
        {
            _worklistExtPoint = new ClearCanvas.Healthcare.Workflow.Reporting.WorklistExtensionPoint();
        }

        #region IReportingWorkflowService Members

        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            ReportingWorklistAssembler assembler = new ReportingWorklistAssembler();
            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, ReportingWorklistItem, List<ReportingWorklistItem>>(
                    GetWorklist(request.WorklistClassName),
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateReportingWorklistItem(item, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetWorklistCountResponse GetWorklistCount(GetWorklistCountRequest request)
        {
            return new GetWorklistCountResponse(GetWorklistCount(request.WorklistClassName));
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(new WorklistItemKey(request.WorklistItem.ProcedureStepRef)));
        }

        [UpdateOperation]
        [OperationEnablement("CanClaimInterpretation")]
        public ClaimInterpretationResponse ClaimInterpretation(ClaimInterpretationRequest request)
        {
            return new ClaimInterpretationResponse(ExecuteOperation(new Operations.ClaimInterpretation(), request.WorklistItem.ProcedureStepRef));
        }

        [UpdateOperation]
        [OperationEnablement("CanStartInterpretation")]
        public StartInterpretationResponse StartInterpretation(StartInterpretationRequest request)
        {
            return new StartInterpretationResponse(ExecuteOperation(new Operations.StartInterpretation(), request.WorklistItem.ProcedureStepRef));
        }

        [UpdateOperation]
        [OperationEnablement("CanStartInterpretation")]
        public EditInterpretationResponse EditInterpretation(EditInterpretationRequest request)
        {
            EntityRef updatedStepRef = ExecuteOperation(new Operations.StartInterpretation(), request.WorklistItem.ProcedureStepRef);
            return new EditInterpretationResponse(updatedStepRef, "Report content not implemented");
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForTranscription")]
        public CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request)
        {
            return new CompleteInterpretationForTranscriptionResponse(ExecuteOperation(new Operations.CompleteInterpretationForTranscription(), request.WorklistItem.ProcedureStepRef));
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForVerification")]
        public CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request)
        {
            return new CompleteInterpretationForVerificationResponse(ExecuteOperation(new Operations.CompleteInterpretationForVerification(), request.WorklistItem.ProcedureStepRef));
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationAndVerify")]
        public CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
        {
            return new CompleteInterpretationAndVerifyResponse(ExecuteOperation(new Operations.CompleteInterpretationAndVerify(), request.WorklistItem.ProcedureStepRef));
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelPendingTranscription")]
        public CancelPendingTranscriptionResponse CancelPendingTranscription(CancelPendingTranscriptionRequest request)
        {
            return new CancelPendingTranscriptionResponse(ExecuteOperation(new Operations.CancelPendingTranscription(), request.WorklistItem.ProcedureStepRef));
        }

        [UpdateOperation]
        [OperationEnablement("CanStartVerification")]
        public StartVerificationResponse StartVerification(StartVerificationRequest request)
        {
            return new StartVerificationResponse(ExecuteOperation(new Operations.StartVerification(), request.WorklistItem.ProcedureStepRef));
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteVerification")]
        public CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request)
        {
            return new CompleteVerificationResponse(ExecuteOperation(new Operations.CompleteVerification(), request.WorklistItem.ProcedureStepRef));
        }

        #endregion

        #region OperationEnablement Helpers

        public bool CanClaimInterpretation(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.ClaimInterpretation(), itemKey);
        }

        public bool CanStartInterpretation(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.StartInterpretation(), itemKey);
        }

        public bool CanCompleteInterpretationForTranscription(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CompleteInterpretationForTranscription(), itemKey);
        }

        public bool CanCompleteInterpretationForVerification(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CompleteInterpretationForVerification(), itemKey);
        }

        public bool CanCompleteInterpretationAndVerify(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CompleteInterpretationAndVerify(), itemKey);
        }

        public bool CanCancelPendingTranscription(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CancelPendingTranscription(), itemKey);
        }

        public bool CanStartVerification(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.StartVerification(), itemKey);
        }

        public bool CanCompleteVerification(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CompleteVerification(), itemKey);
        }

        #endregion

        private EntityRef ExecuteOperation(Operations.ReportingOperation op, EntityRef stepRef)
        {
            // it is extremly important that we get the actual object and not a proxy here
            // if a proxy is returned, then it cannot be cast to a subclass
            // (eg InterpretationStep s = (InterpretationStep)rps; will fail even if we know that rps is an interpretation step)
            ReportingProcedureStep step = (ReportingProcedureStep)PersistenceContext.Load(stepRef, EntityLoadFlags.CheckVersion);
            op.Execute(step, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            // The step will change after the operation, return the updated EntityRef
            ProcedureStep updatedStep = (ProcedureStep) PersistenceContext.Load(stepRef, EntityLoadFlags.Proxy);
            return updatedStep.GetRef();
        }

        private bool CanExecuteOperation(Operations.ReportingOperation op, IWorklistItemKey itemKey)
        {
            if (itemKey is WorklistItemKey)
            {
                ReportingProcedureStep step = (ReportingProcedureStep)PersistenceContext.Load(((WorklistItemKey)itemKey).ReportingProcedureStep);
                return op.CanExecute(step, this.CurrentUserStaff);
            }
            else
            {
                return false;
            }
        }
    }
}
