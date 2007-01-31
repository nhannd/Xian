using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class ReportingWorkflowService : WorkflowServiceBase, IReportingWorkflowService
    {

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

        private ReportingProcedureStep LoadStep(EntityRef<ReportingProcedureStep> stepRef)
        {
            IReportingProcedureStepBroker broker = CurrentContext.GetBroker<IReportingProcedureStepBroker>();

            // it is extremly important that we get the actual object and not a proxy here
            // if a proxy is returned, then it cannot be cast to a subclass
            // (eg InterpretationStep s = (InterpretationStep)rps; will fail even if we know that rps is an interpretation step)
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }

        #region IReportingWorkflowService Members

        [ReadOperation]
        public IList<ReportingWorklistQueryResult> GetWorklist(Type stepClass, ReportingProcedureStepSearchCriteria criteria)
        {
            IReportingWorklistBroker broker = this.CurrentContext.GetBroker<IReportingWorklistBroker>();
            return broker.GetWorklist(stepClass, criteria);
        }

        [UpdateOperation]
        public void ScheduleInterpretation(EntityRef<RequestedProcedure> procedure)
        {
            IRequestedProcedureBroker broker = CurrentContext.GetBroker<IRequestedProcedureBroker>();
            RequestedProcedure rp = broker.Load(procedure, EntityLoadFlags.Proxy);
            InterpretationStep interpretation = new InterpretationStep(rp);
            CurrentContext.Lock(interpretation, DirtyState.New);
        }

        [UpdateOperation]
        public void ClaimInterpretation(EntityRef<InterpretationStep> step)
        {
            ExecuteOperation(LoadStep(step), new Operations.ClaimInterpretation());
        }

        [UpdateOperation]
        public void StartInterpretation(EntityRef<InterpretationStep> step)
        {
            ExecuteOperation(LoadStep(step), new Operations.StartInterpretation());
        }

        [UpdateOperation]
        public void CompleteInterpretationForTranscription(EntityRef<InterpretationStep> step)
        {
            ExecuteOperation(LoadStep(step), new Operations.CompleteInterpretationForTranscription());
        }

        [UpdateOperation]
        public void CompleteInterpretationForVerification(EntityRef<InterpretationStep> step)
        {
            ExecuteOperation(LoadStep(step), new Operations.CompleteInterpretationForVerification());
        }

        [UpdateOperation]
        public void CompleteInterpretationAndVerify(EntityRef<InterpretationStep> step)
        {
            ExecuteOperation(LoadStep(step), new Operations.CompleteInterpretationAndVerify());
        }

        [UpdateOperation]
        public void CancelPendingTranscription(EntityRef<TranscriptionStep> step)
        {
            ExecuteOperation(LoadStep(step), new Operations.CancelPendingTranscription());
        }

        [UpdateOperation]
        public void StartVerification(EntityRef<VerificationStep> step)
        {
            ExecuteOperation(LoadStep(step), new Operations.StartVerification());
        }

        [UpdateOperation]
        public void CompleteVerification(EntityRef<VerificationStep> step)
        {
            ExecuteOperation(LoadStep(step), new Operations.CompleteVerification());
        }

        #endregion
    }
}
