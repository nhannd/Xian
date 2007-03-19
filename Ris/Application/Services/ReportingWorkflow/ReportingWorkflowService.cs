using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Workflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ReportingWorkflowService : WorkflowServiceBase, IReportingWorkflowService
    {
/*
        private InterpretationStep LoadStep(EntityRef<InterpretationStep> stepRef)
        {
            IInterpretationStepBroker broker = CurrentContext.GetBroker<IInterpretationStepBroker>();
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }

        private TranscriptionStep LoadStep(EntityRef<TranscriptionStep> stepRef)
        {
            ITranscriptionStepBroker broker = CurrentContext.GetBroker<ITranscriptionStepBroker>();
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }

        private VerificationStep LoadStep(EntityRef<VerificationStep> stepRef)
        {
            IVerificationStepBroker broker = CurrentContext.GetBroker<IVerificationStepBroker>();
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }
*/
        private ReportingProcedureStep LoadStep(EntityRef stepRef)
        {
            IReportingProcedureStepBroker broker = CurrentContext.GetBroker<IReportingProcedureStepBroker>();

            // it is extremly important that we get the actual object and not a proxy here
            // if a proxy is returned, then it cannot be cast to a subclass
            // (eg InterpretationStep s = (InterpretationStep)rps; will fail even if we know that rps is an interpretation step)
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }

        #region IReportingWorkflowService Members

        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            ReportingWorklistAssembler assembler = new ReportingWorklistAssembler();
            ReportingProcedureStepSearchCriteria criteria = assembler.CreateSearchCriteria(request.SearchCriteria);

            return new GetWorklistResponse(
                CollectionUtils.Map<ReportingWorklistQueryResult, ReportingWorklistItem, List<ReportingWorklistItem>>(
                    PersistenceContext.GetBroker<IReportingWorklistBroker>().GetWorklist(request.StepClass, criteria),
                    delegate(ReportingWorklistQueryResult queryResult)
                    {
                        return assembler.CreateWorklistItem(queryResult);
                    }));
        }

        [UpdateOperation]
        public void ScheduleInterpretation(ScheduleInterpretationRequest request)
        {
            IRequestedProcedureBroker broker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            RequestedProcedure rp = broker.Load(request.ProcedureRef, EntityLoadFlags.Proxy);
            InterpretationStep interpretation = new InterpretationStep(rp);
            CurrentContext.Lock(interpretation, DirtyState.New);
        }

        [UpdateOperation]
        public void ClaimInterpretation(ClaimInterpretationRequest request)
        {
            ExecuteOperation(LoadStep(request.ProcedureStepRef), new Operations.ClaimInterpretation());
        }

        [UpdateOperation]
        public void StartInterpretation(StartInterpretationRequest request)
        {
            ExecuteOperation(LoadStep(request.ProcedureStepRef), new Operations.StartInterpretation());
        }

        [UpdateOperation]
        public void CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request)
        {
            ExecuteOperation(LoadStep(request.ProcedureStepRef), new Operations.CompleteInterpretationForTranscription());
        }

        [UpdateOperation]
        public void CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request)
        {
            ExecuteOperation(LoadStep(request.ProcedureStepRef), new Operations.CompleteInterpretationForVerification());
        }

        [UpdateOperation]
        public void CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
        {
            ExecuteOperation(LoadStep(request.ProcedureStepRef), new Operations.CompleteInterpretationAndVerify());
        }

        [UpdateOperation]
        public void CancelPendingTranscription(CancelPendingTranscriptionRequest request)
        {
            ExecuteOperation(LoadStep(request.ProcedureStepRef), new Operations.CancelPendingTranscription());
        }

        [UpdateOperation]
        public void StartVerification(StartVerificationRequest request)
        {
            ExecuteOperation(LoadStep(request.ProcedureStepRef), new Operations.StartVerification());
        }

        [UpdateOperation]
        public void CompleteVerification(CompleteVerificationRequest request)
        {
            ExecuteOperation(LoadStep(request.ProcedureStepRef), new Operations.CompleteVerification());
        }

        #endregion
    }
}
