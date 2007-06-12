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
            ReportingProcedureStep step = ExecuteOperation(new Operations.ClaimInterpretation(), request.WorklistItem.ProcedureStepRef);
            PersistenceContext.SynchState();
            return new ClaimInterpretationResponse(step.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanStartInterpretation")]
        public StartInterpretationResponse StartInterpretation(StartInterpretationRequest request)
        {
            ReportingProcedureStep step = ExecuteOperation(new Operations.StartInterpretation(), request.WorklistItem.ProcedureStepRef);
            PersistenceContext.SynchState();
            return new StartInterpretationResponse(step.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanStartInterpretation")]
        public EditInterpretationResponse EditInterpretation(EditInterpretationRequest request)
        {
            ReportingProcedureStep step = ExecuteOperation(new Operations.StartInterpretation(), request.WorklistItem.ProcedureStepRef);
            string report = (step.Report == null ? "" : step.Report.ReportContent);

            PersistenceContext.SynchState();
            return new EditInterpretationResponse(step.GetRef(), report);
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForTranscription")]
        public CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request)
        {
            ReportingProcedureStep step = ExecuteOperation(new Operations.CompleteInterpretationForTranscription(), request.WorklistItem.ProcedureStepRef);
            PersistenceContext.SynchState();
            return new CompleteInterpretationForTranscriptionResponse(step.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForVerification")]
        public CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request)
        {
            ReportingProcedureStep step = ExecuteOperation(new Operations.CompleteInterpretationForVerification(), request.WorklistItem.ProcedureStepRef);
            PersistenceContext.SynchState();
            return new CompleteInterpretationForVerificationResponse(step.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationAndVerify")]
        public CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
        {
            ReportingProcedureStep step = ExecuteOperation(new Operations.CompleteInterpretationAndVerify(), request.WorklistItem.ProcedureStepRef);
            PersistenceContext.SynchState();
            return new CompleteInterpretationAndVerifyResponse(step.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelPendingTranscription")]
        public CancelPendingTranscriptionResponse CancelPendingTranscription(CancelPendingTranscriptionRequest request)
        {
            ReportingProcedureStep step = ExecuteOperation(new Operations.CancelPendingTranscription(), request.WorklistItem.ProcedureStepRef);
            PersistenceContext.SynchState();
            return new CancelPendingTranscriptionResponse(step.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanStartVerification")]
        public StartVerificationResponse StartVerification(StartVerificationRequest request)
        {
            ReportingProcedureStep step = ExecuteOperation(new Operations.StartVerification(), request.WorklistItem.ProcedureStepRef);
            PersistenceContext.SynchState();
            return new StartVerificationResponse(step.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanStartVerification")]
        public EditVerificationResponse EditVerification(EditVerificationRequest request)
        {
            ReportingProcedureStep step = ExecuteOperation(new Operations.StartVerification(), request.WorklistItem.ProcedureStepRef);
            string report = (step.Report == null ? "" : step.Report.ReportContent);

            PersistenceContext.SynchState();
            return new EditVerificationResponse(step.GetRef(), report);
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteVerification")]
        public CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request)
        {
            ReportingProcedureStep step = ExecuteOperation(new Operations.CompleteVerification(), request.WorklistItem.ProcedureStepRef);
            PersistenceContext.SynchState();
            return new CompleteVerificationResponse(step.GetRef());
        }

        [UpdateOperation]
        public SaveReportResponse SaveReport(SaveReportRequest request)
        {
            ReportingProcedureStep step = (ReportingProcedureStep)PersistenceContext.Load(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            if (step.Report != null)
            {
                step.Report.ReportContent = request.ReportContent;
            }
            else
            {
                Report report = new Report(step.RequestedProcedure, request.ReportContent);
                PersistenceContext.Lock(report, DirtyState.New);
                step.Report = report;
            }

            PersistenceContext.SynchState();
            return new SaveReportResponse(step.GetRef());
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

        private ReportingProcedureStep ExecuteOperation(Operations.ReportingOperation op, EntityRef stepRef)
        {
            // it is extremly important that we get the actual object and not a proxy here
            // if a proxy is returned, then it cannot be cast to a subclass
            // (eg InterpretationStep s = (InterpretationStep)rps; will fail even if we know that rps is an interpretation step)
            ReportingProcedureStep step = (ReportingProcedureStep)PersistenceContext.Load(stepRef, EntityLoadFlags.CheckVersion);
            op.Execute(step, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
            return step;
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
