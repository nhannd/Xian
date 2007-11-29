#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    [ServiceImplementsContract(typeof(IReportingWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ReportingWorkflowService : WorkflowServiceBase, IReportingWorkflowService
    {
        #region IReportingWorkflowService Members

        [ReadOperation]
        public TextQueryResponse<ReportingWorklistItem> Search(SearchRequest request)
        {
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();
            IReportingWorklistBroker broker = PersistenceContext.GetBroker<IReportingWorklistBroker>();

            WorklistTextQueryHelper<WorklistItem, ReportingWorklistItem> helper =
                new WorklistTextQueryHelper<WorklistItem, ReportingWorklistItem>(
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateReportingWorklistItem(item, PersistenceContext);
                    },
                    delegate(WorklistItemSearchCriteria[] criteria)
                    {
                        return broker.SearchCount(criteria, request.ShowActivOnly);
                    },
                    delegate(WorklistItemSearchCriteria[] criteria, SearchResultPage page)
                    {
                        return broker.Search(criteria, page, request.ShowActivOnly);
                    });

            return helper.Query(request);
        }

        [ReadOperation]
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            WorklistAssembler assembler = new WorklistAssembler();
            return new ListWorklistsResponse(
                CollectionUtils.Map<Worklist, WorklistSummary, List<WorklistSummary>>(
                    this.PersistenceContext.GetBroker<IWorklistBroker>().FindWorklists(this.CurrentUser, WorklistFactory.Instance.GetWorklistClassNames(request.WorklistTokens)),
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
                  ? GetWorklist(request.WorklistType)
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
                            ? GetWorklistCount(request.WorklistType)
                            : GetWorklistCount(request.WorklistRef);

            return new GetWorklistCountResponse(count);
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
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff supervisor = GetSupervisor(interpretation, request.SupervisorRef);

            SaveReportHelper(request, interpretation, supervisor);
            LinkProceduresHelper(request, interpretation);

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
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff supervisor = GetSupervisor(interpretation, request.SupervisorRef);

            if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport) == false && supervisor == null)
                throw new RequestValidationException(SR.ExceptionResidentReportMissingSupervisor);

            SaveReportHelper(request, interpretation, supervisor);
            LinkProceduresHelper(request, interpretation);

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
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.VerifyReport)]
        public CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
        {
            InterpretationStep interpretation = PersistenceContext.Load<InterpretationStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff supervisor = GetSupervisor(interpretation, request.SupervisorRef);

            SaveReportHelper(request, interpretation, supervisor);
            LinkProceduresHelper(request, interpretation);

            if (interpretation.ReportPart == null || String.IsNullOrEmpty(interpretation.ReportPart.Content))
                throw new RequestValidationException(SR.ExceptionVerifyWithNoReport);

            Operations.CompleteInterpretationAndVerify op = new Operations.CompleteInterpretationAndVerify();
            PublicationStep publication = op.Execute(interpretation, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CompleteInterpretationAndVerifyResponse response = new CompleteInterpretationAndVerifyResponse();
            response.InterpretationStepRef = interpretation.GetRef();
            response.PublicationStepRef = publication.GetRef();
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
        [OperationEnablement("CanReviseResidentReport")]
        public ReviseResidentReportResponse ReviseResidentReport(ReviseResidentReportRequest request)
        {
            VerificationStep step = PersistenceContext.Load<VerificationStep>(request.VerificationStepRef, EntityLoadFlags.CheckVersion);

            Operations.ReviseResidentReport op = new Operations.ReviseResidentReport();
            InterpretationStep interpretation = op.Execute(step, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            ReviseResidentReportResponse response = new ReviseResidentReportResponse();
            response.VerificationStepRef = step.GetRef();
            response.InterpretationStepRef = interpretation == null ? null : interpretation.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanStartVerification")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.VerifyReport)]
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
        [PrincipalPermission(SecurityAction.Demand, Role=AuthorityTokens.VerifyReport)]
        public CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request)
        {
            VerificationStep verification = PersistenceContext.Load<VerificationStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);

            SaveReportHelper(request, verification, null);
            LinkProceduresHelper(request, verification);

            if (verification.ReportPart == null || String.IsNullOrEmpty(verification.ReportPart.Content))
                throw new RequestValidationException(SR.ExceptionVerifyWithNoReport);

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
            PublicationStep publication = PersistenceContext.Load<PublicationStep>(request.PublicationStepRef, EntityLoadFlags.CheckVersion);

            Operations.CreateAddendum op = new Operations.CreateAddendum();
            InterpretationStep interpretation = op.Execute(publication, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            CreateAddendumResponse response = new CreateAddendumResponse();
            response.PublicationStepRef = publication.GetRef();
            response.InterpretationStepRef = interpretation.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanReviseUnpublishedReport")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.VerifyReport)]
        public ReviseUnpublishedReportResponse ReviseUnpublishedReport(ReviseUnpublishedReportRequest request)
        {
            PublicationStep publication = PersistenceContext.Load<PublicationStep>(request.PublicationStepRef, EntityLoadFlags.CheckVersion);

            Operations.ReviseUnpublishedReport op = new Operations.ReviseUnpublishedReport();
            VerificationStep verification = op.Execute(publication, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            ReviseUnpublishedReportResponse response = new ReviseUnpublishedReportResponse();
            response.PublicationStepRef = publication.GetRef();
            response.VerificationStepRef = verification.GetRef();
            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanPublishReport")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.VerifyReport)]
        public PublishReportResponse PublishReport(PublishReportRequest request)
        {
            PublicationStep publication = PersistenceContext.Load<PublicationStep>(request.PublicationStepRef, EntityLoadFlags.CheckVersion);

            Operations.PublishReport op = new Operations.PublishReport();
            op.Execute(publication, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));

            PersistenceContext.SynchState();
            return new PublishReportResponse(publication.GetRef());
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
                response.ReportPartIndex = -1;
            }
            else
            {
                response.Report = assembler.CreateReportSummary(step.RequestedProcedure, step.ReportPart.Report, this.PersistenceContext);
                response.ReportPartIndex = step.ReportPart.Index;
            }

            return response;
        }

        [UpdateOperation]
        [OperationEnablement("CanSaveReport")]
        public SaveReportResponse SaveReport(SaveReportRequest request)
        {
            ReportingProcedureStep step = PersistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.CheckVersion);
            Staff supervisor = GetSupervisor(step, request.SupervisorRef);

            SaveReportHelper(request, step, supervisor);
            LinkProceduresHelper(request, step);

            PersistenceContext.SynchState();
            return new SaveReportResponse(step.GetRef());
        }

        [ReadOperation]
        public GetPriorReportsResponse GetPriorReports(GetPriorReportsRequest request)
        {
            IList<Report> priorReports = new List<Report>();
            IPriorReportBroker broker = PersistenceContext.GetBroker<IPriorReportBroker>();

            // if a reporting step was supplied, find priors based on the attached report
            if(request.ReportingProcedureStepRef != null)
            {
                ReportingProcedureStep ps = PersistenceContext.Load<ReportingProcedureStep>(
                    request.ReportingProcedureStepRef, EntityLoadFlags.Proxy);
                if(ps.ReportPart != null)
                {
                    priorReports = broker.GetPriorReports(ps.ReportPart.Report);
                }
            }
            // otherwise if a set of procedures was explicitly supplied, find priors based on those procedures
            else if(request.ProcedureRefs != null && request.ProcedureRefs.Count > 0)
            {
                List<RequestedProcedure> procedures = CollectionUtils.Map<EntityRef, RequestedProcedure>(
                    request.ProcedureRefs,
                    delegate(EntityRef pr) { return PersistenceContext.Load<RequestedProcedure>(pr, EntityLoadFlags.Proxy); });

                priorReports = broker.GetPriorReports(procedures);
            }
            else if(request.PatientRef != null)
            {
                Patient patient = PersistenceContext.Load<Patient>(request.PatientRef, EntityLoadFlags.Proxy);
                priorReports = broker.GetPriorReports(patient);
            }

            // assemble results
            ReportingWorkflowAssembler assembler = new ReportingWorkflowAssembler();
            List<ReportSummary> listSummary = CollectionUtils.Map<Report, ReportSummary>(priorReports,
                delegate(Report report)
                {
                    return assembler.CreateReportSummary(CollectionUtils.FirstElement(report.Procedures), report, this.PersistenceContext);
                });

            return new GetPriorReportsResponse(listSummary);
        }

        [ReadOperation]
        public GetRadiologistListResponse GetRadiologistList(GetRadiologistListRequest request)
        {
            StaffAssembler assembler = new StaffAssembler();

            StaffSearchCriteria criteria = new StaffSearchCriteria();
            criteria.Type.EqualTo(StaffType.PRAD);

            if (String.IsNullOrEmpty(request.SupervisorID) == false)
                criteria.Id.EqualTo(request.SupervisorID);

            IList<Staff> listRadiologists = PersistenceContext.GetBroker<IStaffBroker>().Find(criteria);

            return new GetRadiologistListResponse(
                CollectionUtils.Map<Staff, StaffSummary, List<StaffSummary>>(listRadiologists,
                delegate(Staff staff)
                    {
                        return assembler.CreateStaffSummary(staff, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetLinkableProceduresResponse GetLinkableProcedures(GetLinkableProceduresRequest request)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region OperationEnablement Helpers

        public bool CanClaimInterpretation(WorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.ClaimInterpretation(), itemKey);
        }

        public bool CanStartInterpretation(WorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.StartInterpretation(), itemKey);
        }

        public bool CanCompleteInterpretationForTranscription(WorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CompleteInterpretationForTranscription(), itemKey);
        }

        public bool CanCompleteInterpretationForVerification(WorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CompleteInterpretationForVerification(), itemKey);
        }

        public bool CanCompleteInterpretationAndVerify(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport))
                return false;

            return CanExecuteOperation(new Operations.CompleteInterpretationAndVerify(), itemKey);
        }

        public bool CanCancelReportingStep(WorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.CancelReportingStep(), itemKey);
        }

        public bool CanReviseResidentReport(WorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.ReviseResidentReport(), itemKey);
        }

        public bool CanStartVerification(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport))
                return false;

            return CanExecuteOperation(new Operations.StartVerification(), itemKey);
        }

        public bool CanCompleteVerification(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport))
                return false;

            return CanExecuteOperation(new Operations.CompleteVerification(), itemKey);
        }

        public bool CanCreateAddendum(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport))
                return false;

            return CanExecuteOperation(new Operations.CreateAddendum(), itemKey);
        }

        public bool CanReviseUnpublishedReport(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport))
                return false;

            return CanExecuteOperation(new Operations.ReviseUnpublishedReport(), itemKey);
        }

        public bool CanPublishReport(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport))
                return false;

            return CanExecuteOperation(new Operations.PublishReport(), itemKey);
        }

        public bool CanSaveReport(WorklistItemKey itemKey)
        {
            return CanExecuteOperation(new Operations.SaveReport(), itemKey);
        }

        private bool CanExecuteOperation(Operations.ReportingOperation op, WorklistItemKey itemKey)
        {
            ProcedureStep step = PersistenceContext.Load<ProcedureStep>(itemKey.ProcedureStepRef);

            // for now, all of these operations assume they are operating on a ReportingProcedureStep
            // this may need to change in future
            if(!step.Is<ReportingProcedureStep>())
                return false;
            return op.CanExecute(step.As<ReportingProcedureStep>(), this.CurrentUserStaff);
        }

        #endregion

        private Staff GetSupervisor(ReportingProcedureStep step, EntityRef newSupervisorRef)
        {
            Staff supervisor = newSupervisorRef == null ? null : PersistenceContext.Load<Staff>(newSupervisorRef, EntityLoadFlags.Proxy);

            if (supervisor == null && step.ReportPart != null)
                supervisor = step.ReportPart.Supervisor;

            return supervisor;
        }

        private void SaveReportHelper(SaveReportRequest request, ReportingProcedureStep step, Staff supervisor)
        {
            if (String.IsNullOrEmpty(request.ReportContent) == false)
            {
                Operations.SaveReport saveReportOp = new Operations.SaveReport();
                saveReportOp.Execute(step, request.ReportContent, supervisor, this.PersistenceContext);
            }
        }

        private void LinkProceduresHelper(SaveReportRequest request, ReportingProcedureStep step)
        {
            if(request.LinkProcedureRefs != null && request.LinkProcedureRefs.Count > 0)
            {
                List<RequestedProcedure> procedures = CollectionUtils.Map<EntityRef, RequestedProcedure>(
                    request.LinkProcedureRefs,
                    delegate(EntityRef pr) { return PersistenceContext.Load<RequestedProcedure>(pr); });

                step.ReportPart.Report.LinkProcedures(procedures);
            }
        }

    }
}
