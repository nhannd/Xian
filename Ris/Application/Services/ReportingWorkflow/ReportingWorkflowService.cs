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
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();
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
        public LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request)
        {
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();
            return new LoadWorklistPreviewResponse(assembler.CreateReportingWorklistPreview(request.WorklistItem, this.PersistenceContext));
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
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);

            Operations.ClaimInterpretation op = new Operations.ClaimInterpretation();
            op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            return new ClaimInterpretationResponse(interpretation.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanStartInterpretation")]
        public StartInterpretationResponse StartInterpretation(StartInterpretationRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);

            Operations.StartInterpretation op = new Operations.StartInterpretation();
            op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            return new StartInterpretationResponse(interpretation.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForTranscription")]
        public CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);

            Operations.CompleteInterpretationForTranscription op = new Operations.CompleteInterpretationForTranscription();
            TranscriptionStep transcription = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            CompleteInterpretationForTranscriptionResponse response = new CompleteInterpretationForTranscriptionResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.TranscriptionStepRef = transcription.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForVerification")]
        public CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);

            Operations.CompleteInterpretationForVerification op = new Operations.CompleteInterpretationForVerification();
            VerificationStep verification = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            CompleteInterpretationForVerificationResponse response = new CompleteInterpretationForVerificationResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.VerificationStepRef = verification.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationAndVerify")]
        public CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);

            Operations.CompleteInterpretationAndVerify op = new Operations.CompleteInterpretationAndVerify();
            VerificationStep verification = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            CompleteInterpretationAndVerifyResponse response = new CompleteInterpretationAndVerifyResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.VerificationStepRef = verification.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelPendingTranscription")]
        public CancelPendingTranscriptionResponse CancelPendingTranscription(CancelPendingTranscriptionRequest request)
        {
            TranscriptionStep transcription = PersistenceContext.Load<TranscriptionStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);

            Operations.CancelPendingTranscription op = new Operations.CancelPendingTranscription();
            InterpretationStep interpretation = op.Execute(transcription, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            CancelPendingTranscriptionResponse response = new CancelPendingTranscriptionResponse();
            response.TranscriptionStepRef = transcription.GetRef();
            response.InterpretationStepRef = interpretation.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanStartVerification")]
        public StartVerificationResponse StartVerification(StartVerificationRequest request)
        {
            VerificationStep verification = PersistenceContext.Load<VerificationStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);

            Operations.StartVerification op = new Operations.StartVerification();
            op.Execute(verification, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            return new StartVerificationResponse(verification.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteVerification")]
        public CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request)
        {
            VerificationStep verification = PersistenceContext.Load<VerificationStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);

            Operations.CompleteVerification op = new Operations.CompleteVerification();
            op.Execute(verification, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            return new CompleteVerificationResponse(verification.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanStartAddendum")]
        public StartAddendumResponse StartAddendum(StartAddendumRequest request)
        {
            VerificationStep verification = PersistenceContext.Load<VerificationStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);

            Operations.StartAddendum op = new Operations.StartAddendum();
            AddendumStep addendum = op.Execute(verification, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            StartAddendumResponse response = new StartAddendumResponse();
            response.VerificationStepRef = verification.GetRef();
            response.AddendumStepRef = addendum.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelAddendum")]
        public CancelAddendumResponse CancelAddendum(CancelAddendumRequest request)
        {
            AddendumStep addendum = PersistenceContext.Load<AddendumStep>(request.AddendumStepRef, EntityLoadFlags.CheckVersion);

            Operations.CancelAddendum op = new Operations.CancelAddendum();
            op.Execute(addendum, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            return new CancelAddendumResponse(addendum.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteAddendum")]
        public CompleteAddendumResponse CompleteAddendum(CompleteAddendumRequest request)
        {
            VerificationStep verification = PersistenceContext.Load<VerificationStep>(request.VerificationStepRef, EntityLoadFlags.CheckVersion);
            AddendumStep addendum = PersistenceContext.Load<AddendumStep>(request.AddendumStepRef, EntityLoadFlags.CheckVersion);

            Report report = verification.ReportPart.Report;
            int index = report.Parts.Count + 1;
            addendum.ReportPart = new ReportPart(index.ToString(), request.AddendumContent, report);
            report.Parts.Add(addendum.ReportPart);

            Operations.CompleteAddendum op = new Operations.CompleteAddendum();
            op.Execute(addendum, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            return new CompleteAddendumResponse(addendum.GetRef());
        }

        [ReadOperation]
        public LoadReportForEditResponse LoadReportForEdit(LoadReportForEditRequest request)
        {
            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.WorklistItem.ProcedureStepRef, EntityLoadFlags.CheckVersion);
            string reportPart = (step.ReportPart == null ? "" : step.ReportPart.Content);
            return new LoadReportForEditResponse(reportPart);
        }

        [UpdateOperation]
        public SaveReportResponse SaveReport(SaveReportRequest request)
        {
            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            if (step.ReportPart != null)
            {
                step.ReportPart.Content = request.ReportContent;
            }
            else
            {
                Report report = new Report();
                report.Procedure = step.RequestedProcedure;

                step.ReportPart = new ReportPart("0", request.ReportContent, report);
                report.AddPart(step.ReportPart);

                PersistenceContext.Lock(report, DirtyState.New);
            }
            
            return new SaveReportResponse(step.GetRef());
        }

        [ReadOperation]
        public GetPriorReportResponse GetPriorReport(GetPriorReportRequest request)
        {
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();

            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.Proxy);

            IList<Report> listReports = PersistenceContext.GetBroker<IReportingWorklistBroker>().GetPriorReport(step.RequestedProcedure.Order.Patient);
            List<ReportSummary> listSummary = CollectionUtils.Map<Report, ReportSummary, List<ReportSummary>>(listReports,
                delegate(Report report)
                {
                    return assembler.CreateReportSummary(report);
                });

            return new GetPriorReportResponse(listSummary);
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

        public bool CanStartAddendum(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.StartAddendum(), itemKey);
        }

        public bool CanCancelAddendum(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CancelAddendum(), itemKey);
        }

        public bool CanCompleteAddendum(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CompleteAddendum(), itemKey);
        }

        private bool CanExecuteOperation(Operations.ReportingOperation op, IWorklistItemKey itemKey)
        {
            if (itemKey is WorklistItemKey)
            {
                ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(((WorklistItemKey)itemKey).ReportingProcedureStep);
                return op.CanExecute(step, this.CurrentUserStaff);
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}
