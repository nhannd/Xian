#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using Iesi.Collections.Generic;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    [ServiceImplementsContract(typeof(IReportingWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ReportingWorkflowService : WorkflowServiceBase<ReportingWorklistItem>, IReportingWorkflowService
    {
        #region IReportingWorkflowService Members

        [ReadOperation]
        public TextQueryResponse<ReportingWorklistItem> SearchWorklists(WorklistItemTextQueryRequest request)
        {
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();
            IReportingWorklistItemBroker broker = PersistenceContext.GetBroker<IReportingWorklistItemBroker>();

            return SearchHelper<WorklistItem, ReportingWorklistItem>(request, broker,
                         delegate(WorklistItem item)
                         {
                             return assembler.CreateWorklistItemSummary(item, PersistenceContext);
                         });
        }

        [ReadOperation]
        public QueryWorklistResponse<ReportingWorklistItem> QueryWorklist(QueryWorklistRequest request)
        {
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();

            return QueryWorklistHelper<WorklistItem, ReportingWorklistItem>(request,
                delegate(WorklistItem item)
                {
                    return assembler.CreateWorklistItemSummary(item, this.PersistenceContext);
                });
        }

        [UpdateOperation]
        [OperationEnablement("CanStartInterpretation")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
        public StartInterpretationResponse StartInterpretation(StartInterpretationRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.InterpretationStepRef, EntityLoadFlags.CheckVersion);
            Staff staffAssignedBeforeStart = interpretation.AssignedStaff;

            List<InterpretationStep> linkedInterpretations = new List<InterpretationStep>();
            if (request.LinkedInterpretationStepRefs != null && request.LinkedInterpretationStepRefs.Count > 0)
            {
                linkedInterpretations = CollectionUtils.Map<EntityRef, InterpretationStep>(
                    request.LinkedInterpretationStepRefs,
                    delegate(EntityRef stepRef) { return PersistenceContext.Load<InterpretationStep>(stepRef); });
            }

            Operations.StartInterpretation op = new Operations.StartInterpretation();
            op.Execute(interpretation, this.CurrentUserStaff, linkedInterpretations, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            return new StartInterpretationResponse(interpretation.GetRef(),
                staffAssignedBeforeStart == null ? null : interpretation.AssignedStaff.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanStartTranscriptionReview")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
        public StartTranscriptionReviewResponse StartTranscriptionReview(StartTranscriptionReviewRequest request)
        {
            TranscriptionReviewStep transcriptionReviewStep = PersistenceContext.Load<TranscriptionReviewStep>(request.TranscriptionReviewStepRef, EntityLoadFlags.CheckVersion);

            Operations.StartTranscriptionReview op = new Operations.StartTranscriptionReview();
            op.Execute(transcriptionReviewStep, this.CurrentUserStaff);

            PersistenceContext.SynchState();
            return new StartTranscriptionReviewResponse(transcriptionReviewStep.GetRef());
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForTranscription")]
        public CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request)
        {
            ReportingProcedureStep interpretation = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff supervisor = ResolveSupervisor(interpretation, request.SupervisorRef);

            SaveReportHelper(request.ReportPartExtendedProperties, interpretation, supervisor, true);

            ValidateReportTextExists(interpretation);

            Operations.CompleteInterpretationForTranscription op = new Operations.CompleteInterpretationForTranscription();
            ReportingProcedureStep nextStep = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CompleteInterpretationForTranscriptionResponse response = new CompleteInterpretationForTranscriptionResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.TranscriptionStepRef = nextStep.GetRef();
            return response;
        }

        // TODO: change this to submit for review
        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationForVerification")]
        public CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request)
        {
            ReportingProcedureStep interpretation = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff supervisor = ResolveSupervisor(interpretation, request.SupervisorRef);

            SaveReportHelper(request.ReportPartExtendedProperties, interpretation, supervisor, true);

            ValidateReportTextExists(interpretation);

            Operations.CompleteInterpretationForVerification op = new Operations.CompleteInterpretationForVerification();
            ReportingProcedureStep nextStep = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CompleteInterpretationForVerificationResponse response = new CompleteInterpretationForVerificationResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.VerificationStepRef = nextStep.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteInterpretationAndVerify")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Verify)]
        public CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
        {
            ReportingProcedureStep interpretation = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff supervisor = ResolveSupervisor(interpretation, request.SupervisorRef);

            SaveReportHelper(request.ReportPartExtendedProperties, interpretation, supervisor, true);

            ValidateReportTextExists(interpretation);

            Operations.CompleteInterpretationAndVerify op = new Operations.CompleteInterpretationAndVerify();
            ReportingProcedureStep nextStep = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CompleteInterpretationAndVerifyResponse response = new CompleteInterpretationAndVerifyResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.PublicationStepRef = nextStep.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelReportingStep")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
        public CancelReportingStepResponse CancelReportingStep(CancelReportingStepRequest request)
        {
            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff reassignStaff = request.ReassignedToStaff != null ? PersistenceContext.Load<Staff>(request.ReassignedToStaff, EntityLoadFlags.CheckVersion) : null;

            // demand authority token if trying to cancel a step that is assigned to someone else
            if (step.AssignedStaff != null && !Equals(step.AssignedStaff, this.CurrentUserStaff))
            {
                PrincipalPermission permission = new PrincipalPermission(null, AuthorityTokens.Workflow.Report.Cancel);
                permission.Demand();
            }

            Operations.CancelReportingStep op = new Operations.CancelReportingStep();
            List<InterpretationStep> scheduledInterpretations = op.Execute(step, this.CurrentUserStaff, reassignStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();

            return new CancelReportingStepResponse(step.GetRef(),
                CollectionUtils.Map<InterpretationStep, EntityRef>(scheduledInterpretations,
                    delegate(InterpretationStep s) { return s.GetRef(); }));
        }

        [UpdateOperation]
        [OperationEnablement("CanReviseResidentReport")]
        public ReviseResidentReportResponse ReviseResidentReport(ReviseResidentReportRequest request)
        {
            VerificationStep step = PersistenceContext.Load<VerificationStep>(request.VerificationStepRef, EntityLoadFlags.CheckVersion);

            Operations.ReviseResidentReport op = new Operations.ReviseResidentReport();
            InterpretationStep interpretation = op.Execute(step, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            return new ReviseResidentReportResponse(GetWorklistItemSummary(interpretation));
        }

        [UpdateOperation]
        [OperationEnablement("CanStartVerification")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Verify)]
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
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Verify)]
        public CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request)
        {
            VerificationStep verification = PersistenceContext.Load<VerificationStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff supervisor = ResolveSupervisor(verification, request.SupervisorRef);

            SaveReportHelper(request.ReportPartExtendedProperties, verification, supervisor, true);

            ValidateReportTextExists(verification);

            Operations.CompleteVerification op = new Operations.CompleteVerification();
            PublicationStep publication = op.Execute(verification, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();

            CompleteVerificationResponse response = new CompleteVerificationResponse();
            response.VerificationStepRef = verification.GetRef();
            response.PublicationStepRef = publication.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanCreateAddendum")]
        public CreateAddendumResponse CreateAddendum(CreateAddendumRequest request)
        {
            Procedure procedure = PersistenceContext.Load<Procedure>(request.ProcedureRef);

            Operations.CreateAddendum op = new Operations.CreateAddendum();
            InterpretationStep interpretation = op.Execute(procedure, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
            PersistenceContext.SynchState();

            CreateAddendumResponse response = new CreateAddendumResponse();
            response.ReportingWorklistItem = GetWorklistItemSummary(interpretation); ;
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanReviseUnpublishedReport")]
        public ReviseUnpublishedReportResponse ReviseUnpublishedReport(ReviseUnpublishedReportRequest request)
        {
            PublicationStep publication = PersistenceContext.Load<PublicationStep>(request.PublicationStepRef, EntityLoadFlags.CheckVersion);

            Operations.ReviseUnpublishedReport op = new Operations.ReviseUnpublishedReport();
            VerificationStep verification = op.Execute(publication, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();

            return new ReviseUnpublishedReportResponse(GetWorklistItemSummary(verification));
        }

        [UpdateOperation]
        [OperationEnablement("CanPublishReport")]
        public PublishReportResponse PublishReport(PublishReportRequest request)
        {
            PublicationStep publication = PersistenceContext.Load<PublicationStep>(request.PublicationStepRef, EntityLoadFlags.CheckVersion);

            Operations.PublishReport op = new Operations.PublishReport();
            op.Execute(publication, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            return new PublishReportResponse(publication.GetRef());
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
        public LoadReportForEditResponse LoadReportForEdit(LoadReportForEditRequest request)
        {
            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            ReportAssembler assembler = new ReportAssembler();
            OrderAssembler orderAssembler = new OrderAssembler();

            LoadReportForEditResponse response = new LoadReportForEditResponse(
                assembler.CreateReportDetail(step.ReportPart.Report, false, this.PersistenceContext),
                step.ReportPart.Index,
                orderAssembler.CreateOrderDetail(step.Procedure.Order, PersistenceContext, false, false, false, null, false,
                                                 false, true));

            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanSaveReport")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Create)]
        public SaveReportResponse SaveReport(SaveReportRequest request)
        {
            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff supervisor = ResolveSupervisor(step, request.SupervisorRef);

			// saving a draft does not require supervisor validation
            SaveReportHelper(request.ReportPartExtendedProperties, step, supervisor, false);

            PersistenceContext.SynchState();
            return new SaveReportResponse(step.GetRef());
        }

        [ReadOperation]
        public GetPriorsResponse GetPriors(GetPriorsRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            if (request.PatientRef == null && request.OrderRef == null && request.ReportRef == null)
                throw new ArgumentException("Either PatientRef or ReportingProcedureStepRef must be non-null");

            HashedSet<Report> priorReports = new HashedSet<Report>();

            //TODO: ensure IPriorReportBroker uses fetch joins to get report, procedures, order, etc in one go
            IPriorReportBroker broker = PersistenceContext.GetBroker<IPriorReportBroker>();

            // if a patient was supplied, find all reports for the patient
            if (request.PatientRef != null)
            {
                Patient patient = PersistenceContext.Load<Patient>(request.PatientRef, EntityLoadFlags.Proxy);
                priorReports.AddAll(broker.GetPriors(patient));
            }
            // if an order was supplied, find relevant priors for the order
            else if (request.OrderRef != null)
            {
                Order order = this.PersistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.Proxy);
                priorReports.AddAll(broker.GetPriors(order));
            }
            // if a report was supplied, find relevent priors
            else if (request.ReportRef != null)
            {
                Report report = this.PersistenceContext.Load<Report>(request.ReportRef, EntityLoadFlags.Proxy);
                priorReports.AddAll(broker.GetPriors(report));
            }

            // assemble results
            ProcedureTypeAssembler rptAssembler = new ProcedureTypeAssembler();
            DiagnosticServiceAssembler dsAssembler = new DiagnosticServiceAssembler();
            List<PriorProcedureSummary> priorSummaries = new List<PriorProcedureSummary>();
            foreach (Report priorReport in priorReports)
            {
                foreach (Procedure procedure in priorReport.Procedures)
                {
                    // Note: we use the ProcedureCheckin.CheckOutTime as the PerformedDate
                    // because it is the closest to the end of modality procedure step completion time.
                    // However, if we change the definition of CheckOutTime in the future, this won't be accurate
                    PriorProcedureSummary summary = new PriorProcedureSummary(
                        procedure.Order.GetRef(),
                        procedure.GetRef(),
                        priorReport.GetRef(),
                        procedure.Order.AccessionNumber,
                        dsAssembler.CreateSummary(procedure.Order.DiagnosticService),
                        rptAssembler.CreateSummary(procedure.Type),
                        procedure.Portable,
                        EnumUtils.GetEnumValueInfo(procedure.Laterality, PersistenceContext),
                        EnumUtils.GetEnumValueInfo(priorReport.Status, PersistenceContext),
                        procedure.ProcedureCheckIn.CheckOutTime);

                    priorSummaries.Add(summary);
                }
            }
            return new GetPriorsResponse(priorSummaries);
        }

        [ReadOperation]
        public GetLinkableInterpretationsResponse GetLinkableInterpretations(GetLinkableInterpretationsRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.InterpretationStepRef, "InterpretationStepRef");

            InterpretationStep step = PersistenceContext.Load<InterpretationStep>(request.InterpretationStepRef, EntityLoadFlags.Proxy);

            IReportingWorklistItemBroker broker = PersistenceContext.GetBroker<IReportingWorklistItemBroker>();
            IList<InterpretationStep> candidateSteps = broker.GetLinkedInterpretationCandidates(step, this.CurrentUserStaff);

            // if any candidate steps were found, need to convert them to worklist items
            IList<WorklistItem> worklistItems;
            if (candidateSteps.Count > 0)
            {
                // because CLR does not support List co-variance, need to map to a list of the more general type (this seems silly!)
                List<ReportingProcedureStep> reportingSteps =
                    CollectionUtils.Map<InterpretationStep, ReportingProcedureStep>(
                        candidateSteps, delegate(InterpretationStep s) { return s; });

                worklistItems = broker.GetWorklistItems(reportingSteps);
            }
            else
            {
                worklistItems = new List<WorklistItem>();
            }

            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();
            return new GetLinkableInterpretationsResponse(
                CollectionUtils.Map<WorklistItem, ReportingWorklistItem>(worklistItems,
                delegate(WorklistItem item)
                {
                    return assembler.CreateWorklistItemSummary(item, PersistenceContext);
                }));
        }

        [UpdateOperation]
        [OperationEnablement("CanSendReportToQueue")]
        public SendReportToQueueResponse SendReportToQueue(SendReportToQueueRequest request)
        {
            Procedure procedure = PersistenceContext.Load<Procedure>(request.ProcedureRef);

            foreach (PublishRecipientDetail detail in request.Recipients)
            {
                WorkQueueItem item = MailFaxWorkQueueItem.Create(
                    procedure.Order.AccessionNumber,
                    procedure.ActiveReport.GetRef(),
                    detail.PractitionerRef,
                    detail.ContactPointRef);

                PersistenceContext.Lock(item, DirtyState.New);
            }

            return new SendReportToQueueResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanReassignProcedureStep")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Report.Reassign)]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Reassign)]
        public ReassignProcedureStepResponse ReassignProcedureStep(ReassignProcedureStepRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.ProcedureStepRef, "ProcedureStepRef");
            Platform.CheckMemberIsSet(request.ReassignedRadiologistRef, "ReassignedRadiologistRef");

            ProcedureStep step = PersistenceContext.Load<ProcedureStep>(request.ProcedureStepRef, EntityLoadFlags.Proxy);
            Staff radiologist = PersistenceContext.Load<Staff>(request.ReassignedRadiologistRef, EntityLoadFlags.Proxy);

            ProcedureStep newStep;

            if (step is ReportingProcedureStep && !request.KeepReportPart)
            {
                ReportingProcedureStep oldReportingStep = (ReportingProcedureStep)step;
                if (oldReportingStep.ReportPart != null)
                    oldReportingStep.ReportPart.Cancel();

                newStep = step.Reassign(radiologist);
                ReportingProcedureStep newReportingStep = (ReportingProcedureStep)newStep;
                newReportingStep.ReportPart = null;
            }
            else
            {
                newStep = step.Reassign(radiologist);
            }

            PersistenceContext.SynchState();

            return new ReassignProcedureStepResponse(newStep.GetRef());
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Downtime.RecoveryOperations)]
        public CompleteDowntimeProcedureResponse CompleteDowntimeProcedure(CompleteDowntimeProcedureRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.ProcedureRef, "ProcedureRef");

            Procedure procedure = PersistenceContext.Load<Procedure>(request.ProcedureRef);

            if (request.ReportProvided)
            {
                Platform.CheckMemberIsSet(request.InterpreterRef, "InterpreterRef");

                Staff interpreter = PersistenceContext.Load<Staff>(request.InterpreterRef);
                Staff transcriptionist = request.TranscriptionistRef == null ? null : PersistenceContext.Load<Staff>(request.TranscriptionistRef);

                // find the relevant interpretation step for this procedure
                InterpretationStep interpStep = procedure.GetProcedureStep(delegate(ProcedureStep ps)
                    { return ps.Is<InterpretationStep>() && ps.State == ActivityStatus.SC; }).As<InterpretationStep>();

                // ideally this should not happen, but what do we do if it does?
                if (interpStep == null)
                    throw new RequestValidationException("Report cannot be submitted for this procedure.  It may have been submitted previously.");

                // start interpretation, using specified interpreter
                // the report will end up in their drafts folder
                Operations.StartInterpretation startOp = new Operations.StartInterpretation();
                startOp.Execute(interpStep, interpreter, new List<InterpretationStep>(), new PersistentWorkflow(this.PersistenceContext));

                // save the report data
                SaveReportHelper(request.ReportPartExtendedProperties, interpStep, null, false);

                ValidateReportTextExists(interpStep);

                // set the transcriptionist if known
                interpStep.ReportPart.Transcriber = transcriptionist;
            }

            // flip the downtime mode switch
            procedure.DowntimeRecoveryMode = false;

            return new CompleteDowntimeProcedureResponse();
        }

        #endregion

        #region OperationEnablement Helpers

        public bool CanStartInterpretation(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Create))
                return false;
            return CanExecuteOperation(new Operations.StartInterpretation(), itemKey);
        }

        public bool CanStartTranscriptionReview(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Create))
                return false;
            return CanExecuteOperation(new Operations.StartTranscriptionReview(), itemKey);
        }

        public bool CanCompleteInterpretationForTranscription(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Create))
                return false;
            return CanExecuteOperation(new Operations.CompleteInterpretationForTranscription(), itemKey);
        }

        public bool CanCompleteInterpretationForVerification(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Create))
                return false;

            return CanExecuteOperation(new Operations.CompleteInterpretationForVerification(), itemKey);
        }

        public bool CanCompleteInterpretationAndVerify(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify))
                return false;

            return CanExecuteOperation(new Operations.CompleteInterpretationAndVerify(), itemKey);
        }

        public bool CanCancelReportingStep(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Create))
                return false;

            // if there is no proc step ref, operation is not available
            if (itemKey.ProcedureStepRef == null)
                return false;

            ProcedureStep step = PersistenceContext.Load<ProcedureStep>(itemKey.ProcedureStepRef);

            // cannot cancel a step that is assigned to someone else without the authority token
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Cancel) &&
                (step.AssignedStaff != null && !Equals(step.AssignedStaff, this.CurrentUserStaff)))
                return false;

            return CanExecuteOperation(new Operations.CancelReportingStep(), itemKey, true);
        }

        public bool CanReviseResidentReport(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.SubmitForReview))
                return false;

            return CanExecuteOperation(new Operations.ReviseResidentReport(), itemKey);
        }

        public bool CanStartVerification(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify))
                return false;

            // If the submit for review token is present, do not enable verification, defer to 
            // revise report.  This ensures items submitted with or without a supervisor have a
            // consistent set of operations.
            return CanExecuteOperation(new Operations.StartVerification(), itemKey, true);
        }

        public bool CanCompleteVerification(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Verify))
                return false;

            // If the submit for review token is present, do not enable verification, defer to 
            // revise report.  This ensures items submitted with or without a supervisor have a
            // consistent set of operations.
            return CanExecuteOperation(new Operations.CompleteVerification(), itemKey, true);
        }

        public bool CanCreateAddendum(WorklistItemKey itemKey)
        {
            // special case: procedure step not known, but procedure is
            if (itemKey.ProcedureRef != null)
            {
                Procedure procedure = PersistenceContext.Load<Procedure>(itemKey.ProcedureRef);
                return (new Operations.CreateAddendum()).CanExecute(procedure, CurrentUserStaff);
            }
            return false;
        }

        public bool CanReviseUnpublishedReport(WorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.ReviseUnpublishedReport(), itemKey);
        }

        public bool CanPublishReport(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Development.TestPublishReport))
                return false;
            return CanExecuteOperation(new Operations.PublishReport(), itemKey);
        }

        public bool CanSaveReport(WorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.SaveReport(), itemKey);
        }

        public bool CanReassignProcedureStep(WorklistItemKey itemKey)
        {
            if (itemKey.ProcedureStepRef == null)
                return false;

            ProcedureStep step = PersistenceContext.Load<ProcedureStep>(itemKey.ProcedureStepRef);
            if (step is ProtocolProcedureStep)
                return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Reassign);
            else if (step is ReportingProcedureStep)
                return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.Reassign)
                    && !(step is VerificationStep)
                    && !(step is PublicationStep);
            else
                return false;
        }

        public bool CanSendReportToQueue(WorklistItemKey itemKey)
        {
            // does the item have a procedure ref, or is it just a patient?
            if (itemKey.ProcedureRef == null)
                return false;

            // does the procedure have an active report
            Procedure procedure = PersistenceContext.Load<Procedure>(itemKey.ProcedureRef);
            if (procedure.ActiveReport == null)
                return false;

            return true;
        }

        private bool CanExecuteOperation(Operations.ReportingOperation op, WorklistItemKey itemKey)
        {
            return CanExecuteOperation(op, itemKey, false);
        }

        private bool CanExecuteOperation(Operations.ReportingOperation op, WorklistItemKey itemKey, bool disableIfSubmitForReview)
        {
            // if there is no proc step ref, operation is not available
            if (itemKey.ProcedureStepRef == null)
                return false;

            ProcedureStep step = PersistenceContext.Load<ProcedureStep>(itemKey.ProcedureStepRef);

            // for now, all of these operations assume they are operating on a ReportingProcedureStep
            // this may need to change in future
            if (!step.Is<ReportingProcedureStep>())
                return false;

            // Special Case:
            // If the user has the SubmitForReview token and the step is unassigned, disable the operation
            if (disableIfSubmitForReview
                && step.AssignedStaff == null
                && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.SubmitForReview))
            {
                return false;
            }

            return op.CanExecute(step.As<ReportingProcedureStep>(), this.CurrentUserStaff);
        }

        #endregion

        protected override object GetWorkItemKey(ReportingWorklistItem item)
        {
            return new WorklistItemKey(item.ProcedureStepRef, item.ProcedureRef);
        }

        /// <summary>
        /// Get the supervisor, using the new supervisor if supplied, otherwise using an existing supervisor if found.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="newSupervisorRef"></param>
        /// <returns></returns>
        private Staff ResolveSupervisor(ReportingProcedureStep step, EntityRef newSupervisorRef)
        {
            Staff supervisor = newSupervisorRef == null ? null : PersistenceContext.Load<Staff>(newSupervisorRef, EntityLoadFlags.Proxy);

            if (supervisor == null && step.ReportPart != null)
                supervisor = step.ReportPart.Supervisor;


            return supervisor;
        }

        /// <summary>
        /// Saves the report, and validates that a supervisor is present if the current user does not have 'unsupervised reporting' permissions.
        /// </summary>
        /// <param name="reportPartExtendedProperties"></param>
        /// <param name="step"></param>
        /// <param name="supervisor"></param>
        /// <param name="supervisorValidationRequired"></param>
        private void SaveReportHelper(Dictionary<string, string> reportPartExtendedProperties, ReportingProcedureStep step, Staff supervisor, bool supervisorValidationRequired)
        {
            if (supervisorValidationRequired 
                && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Report.OmitSupervisor) == false 
                && supervisor == null)
            {
                throw new SupervisorValidationException();
            }

            if (reportPartExtendedProperties == null)
                return;

            Operations.SaveReport saveReportOp = new Operations.SaveReport();
            saveReportOp.Execute(step, reportPartExtendedProperties, supervisor, this.PersistenceContext);
        }

        private void ValidateReportTextExists(ReportingProcedureStep step)
        {
            string content;
            if (step.ReportPart == null || step.ReportPart.ExtendedProperties == null
                || !step.ReportPart.ExtendedProperties.TryGetValue(ReportPartDetail.ReportContentKey, out content)
                || string.IsNullOrEmpty(content))
            {
                throw new RequestValidationException(SR.ExceptionVerifyWithNoReport);
            }
        }

        private ReportingWorklistItem GetWorklistItemSummary(ReportingProcedureStep reportingProcedureStep)
        {
            IList<ReportingProcedureStep> procedureSteps = new List<ReportingProcedureStep>();
            procedureSteps.Add(reportingProcedureStep);

            IList<WorklistItem> items = this.PersistenceContext.GetBroker<IReportingWorklistItemBroker>().GetWorklistItems(procedureSteps);
            return new ReportingWorkflowAssembler().CreateWorklistItemSummary(CollectionUtils.FirstElement(items), this.PersistenceContext);
        }
    }
}
