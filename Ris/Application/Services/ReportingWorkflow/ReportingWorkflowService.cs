using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

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
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            WorklistAssembler assembler = new WorklistAssembler();
            return new ListWorklistsResponse(
                CollectionUtils.Map<Worklist, WorklistSummary, List<WorklistSummary>>(
                    this.PersistenceContext.GetBroker<IWorklistBroker>().FindAllReportingWorklists(this.CurrentUser),
                    delegate(Worklist worklist)
                    {
                        return assembler.GetWorklistSummary(worklist);
                    }));
        }


        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();

            IList items = request.WorklistRef == null
                  ? GetWorklist(request.WorklistClassName)
                  : GetWorklist(request.WorklistRef);

            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, ReportingWorklistItem, List<ReportingWorklistItem>>(
                    items,
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateReportingWorklistItem(item, this.PersistenceContext);
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
        public LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request)
        {
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();
            return new LoadWorklistPreviewResponse(assembler.CreateReportingWorklistPreview(request.WorklistItem, this.PersistenceContext));
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(new WorklistItemKey(request.ProcedureStepRef)));
        }

        [UpdateOperation]
        [OperationEnablement("CanClaimInterpretation")]
        public ClaimInterpretationResponse ClaimInterpretation(ClaimInterpretationRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.InterpretationStepRef, EntityLoadFlags.CheckVersion);

            Operations.ClaimInterpretation op = new Operations.ClaimInterpretation();
            op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            return new ClaimInterpretationResponse(interpretation.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanStartInterpretation")]
        public StartInterpretationResponse StartInterpretation(StartInterpretationRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.InterpretationStepRef, EntityLoadFlags.CheckVersion);

            Operations.StartInterpretation op = new Operations.StartInterpretation();
            op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            return new StartInterpretationResponse(interpretation.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForTranscription")]
        public CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.InterpretationStepRef, EntityLoadFlags.CheckVersion);

            if (String.IsNullOrEmpty(request.ReportContent) == false)
            {
                SaveReportHelper(interpretation, request.ReportContent);
            }

            Operations.CompleteInterpretationForTranscription op = new Operations.CompleteInterpretationForTranscription();
            TranscriptionStep transcription = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CompleteInterpretationForTranscriptionResponse response = new CompleteInterpretationForTranscriptionResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.TranscriptionStepRef = transcription.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForVerification")]
        public CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.InterpretationStepRef, EntityLoadFlags.CheckVersion);

            if (String.IsNullOrEmpty(request.ReportContent) == false)
            {
                SaveReportHelper(interpretation, request.ReportContent);
            }

            Operations.CompleteInterpretationForVerification op = new Operations.CompleteInterpretationForVerification();
            VerificationStep verification = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CompleteInterpretationForVerificationResponse response = new CompleteInterpretationForVerificationResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.VerificationStepRef = verification.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationAndVerify")]
        public CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.InterpretationStepRef, EntityLoadFlags.CheckVersion);

            if (String.IsNullOrEmpty(request.ReportContent) == false)
            {
                SaveReportHelper(interpretation, request.ReportContent);
            }

            if (interpretation.ReportPart == null || String.IsNullOrEmpty(interpretation.ReportPart.Content))
                throw new RequestValidationException(SR.ExceptionVerifyWithNoReport);

            Operations.CompleteInterpretationAndVerify op = new Operations.CompleteInterpretationAndVerify();
            VerificationStep verification = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CompleteInterpretationAndVerifyResponse response = new CompleteInterpretationAndVerifyResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.VerificationStepRef = verification.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelReportingStep")]
        public CancelReportingStepResponse CancelReportingStep(CancelReportingStepRequest request)
        {
            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);

            Operations.CancelReportingStep op = new Operations.CancelReportingStep();
            InterpretationStep interpretation = op.Execute(step, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CancelReportingStepResponse response = new CancelReportingStepResponse();
            response.ReportingStepRef = step.GetRef();
            response.InterpretationStepRef = interpretation == null ? null : interpretation.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanStartVerification")]
        public StartVerificationResponse StartVerification(StartVerificationRequest request)
        {
            VerificationStep verification = PersistenceContext.Load<VerificationStep>(request.VerificationStepRef, EntityLoadFlags.CheckVersion);

            Operations.StartVerification op = new Operations.StartVerification();
            op.Execute(verification, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            return new StartVerificationResponse(verification.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteVerification")]
        public CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request)
        {
            VerificationStep verification = PersistenceContext.Load<VerificationStep>(request.VerificationStepRef, EntityLoadFlags.CheckVersion);

            if (verification.ReportPart == null || String.IsNullOrEmpty(verification.ReportPart.Content))
                throw new RequestValidationException(SR.ExceptionVerifyWithNoReport);

            Operations.CompleteVerification op = new Operations.CompleteVerification();
            op.Execute(verification, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            return new CompleteVerificationResponse(verification.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCreateAddendum")]
        public CreateAddendumResponse CreateAddendum(CreateAddendumRequest request)
        {
            VerificationStep verification = PersistenceContext.Load<VerificationStep>(request.VerificationStepRef, EntityLoadFlags.CheckVersion);

            Operations.CreateAddendum op = new Operations.CreateAddendum();
            InterpretationStep interpretation = op.Execute(verification, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CreateAddendumResponse response = new CreateAddendumResponse();
            response.VerificationStepRef = verification.GetRef();
            response.InterpretationStepRef = interpretation.GetRef();
            return response;
        }

        [ReadOperation]
        public LoadReportForEditResponse LoadReportForEdit(LoadReportForEditRequest request)
        {
            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();

            LoadReportForEditResponse response = new LoadReportForEditResponse();
            if (step.ReportPart == null)
            {
                response.Report = assembler.CreateReportSummary(step.RequestedProcedure, null, this.PersistenceContext);
                response.ReportPartIndex = 0;
            }
            else
            {
                response.Report = assembler.CreateReportSummary(step.RequestedProcedure, step.ReportPart.Report, this.PersistenceContext);
                response.ReportPartIndex = int.Parse(step.ReportPart.Index);
            }

            return response;
        }

        [UpdateOperation]
        public SaveReportResponse SaveReport(SaveReportRequest request)
        {
            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);

            SaveReportHelper(step, request.ReportContent);

            PersistenceContext.SynchState();
            return new SaveReportResponse(step.GetRef());
        }

        private void SaveReportHelper(ReportingProcedureStep step, string reportContent)
        {
            if (step.ReportPart != null)
            {
                step.ReportPart.Content = reportContent;
            }
            else
            {
                Report report = new Report();
                report.Procedure = step.RequestedProcedure;
                step.ReportPart = report.AddPart(reportContent);

                PersistenceContext.Lock(report, DirtyState.New);
            }
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
                    return assembler.CreateReportSummary(report.Procedure, report, this.PersistenceContext);
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

        public bool CanCancelReportingStep(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CancelReportingStep(), itemKey);
        }

        public bool CanStartVerification(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.StartVerification(), itemKey);
        }

        public bool CanCompleteVerification(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CompleteVerification(), itemKey);
        }

        public bool CanCreateAddendum(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CreateAddendum(), itemKey);
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
