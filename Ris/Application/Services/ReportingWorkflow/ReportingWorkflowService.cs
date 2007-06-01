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
        public void ClaimInterpretation(ClaimInterpretationRequest request)
        {
            ExecuteOperation(new Operations.ClaimInterpretation(), request.WorklistItem.ProcedureStepRef);
        }

        [UpdateOperation]
        [OperationEnablement("CanStartInterpretation")]
        public void StartInterpretation(StartInterpretationRequest request)
        {
            ExecuteOperation(new Operations.StartInterpretation(), request.WorklistItem.ProcedureStepRef);
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForTranscription")]
        public void CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request)
        {
            ExecuteOperation(new Operations.CompleteInterpretationForTranscription(), request.WorklistItem.ProcedureStepRef);
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForVerification")]
        public void CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request)
        {
            ExecuteOperation(new Operations.CompleteInterpretationForVerification(), request.WorklistItem.ProcedureStepRef);
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationAndVerify")]
        public void CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
        {
            ExecuteOperation(new Operations.CompleteInterpretationAndVerify(), request.WorklistItem.ProcedureStepRef);
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelPendingTranscription")]
        public void CancelPendingTranscription(CancelPendingTranscriptionRequest request)
        {
            ExecuteOperation(new Operations.CancelPendingTranscription(), request.WorklistItem.ProcedureStepRef);
        }

        [UpdateOperation]
        [OperationEnablement("CanStartVerification")]
        public void StartVerification(StartVerificationRequest request)
        {
            ExecuteOperation(new Operations.StartVerification(), request.WorklistItem.ProcedureStepRef);
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteVerification")]
        public void CompleteVerification(CompleteVerificationRequest request)
        {
            ExecuteOperation(new Operations.CompleteVerification(), request.WorklistItem.ProcedureStepRef);
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

        private void ExecuteOperation(Operations.ReportingOperation op, EntityRef stepRef)
        {
            // it is extremly important that we get the actual object and not a proxy here
            // if a proxy is returned, then it cannot be cast to a subclass
            // (eg InterpretationStep s = (InterpretationStep)rps; will fail even if we know that rps is an interpretation step)
            ReportingProcedureStep step = (ReportingProcedureStep)PersistenceContext.Load(stepRef, EntityLoadFlags.CheckVersion);
            op.Execute(step, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
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
