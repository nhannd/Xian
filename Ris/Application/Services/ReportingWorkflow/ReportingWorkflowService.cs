using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Workflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    [ServiceImplementsContract(typeof(IReportingWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ReportingWorkflowService : WorkflowServiceBase, IReportingWorkflowService
    {
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
                        return assembler.CreateReportingWorklistItem(queryResult);
                    }));
        }

        [UpdateOperation]
        public void ScheduleInterpretation(ScheduleInterpretationRequest request)
        {
            IRequestedProcedureBroker broker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            RequestedProcedure rp = broker.Load(request.ProcedureRef, EntityLoadFlags.Proxy);
            InterpretationStep interpretation = new InterpretationStep(rp);
            PersistenceContext.Lock(interpretation, DirtyState.New);
        }

        [UpdateOperation]
        public void ClaimInterpretation(ClaimInterpretationRequest request)
        {
            //ExecuteOperation(request.WorklistItem, new Operations.ClaimInterpretation());
        }

        [UpdateOperation]
        public void StartInterpretation(StartInterpretationRequest request)
        {
            //ExecuteOperation(request.WorklistItem, new Operations.StartInterpretation());
        }

        [UpdateOperation]
        public void CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request)
        {
            //ExecuteOperation(request.WorklistItem, new Operations.CompleteInterpretationForTranscription());
        }

        [UpdateOperation]
        public void CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request)
        {
            //ExecuteOperation(request.WorklistItem, new Operations.CompleteInterpretationForVerification());
        }

        [UpdateOperation]
        public void CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request)
        {
            //ExecuteOperation(request.WorklistItem, new Operations.CompleteInterpretationAndVerify());
        }

        [UpdateOperation]
        public void CancelPendingTranscription(CancelPendingTranscriptionRequest request)
        {
            //ExecuteOperation(request.WorklistItem, new Operations.CancelPendingTranscription());
        }

        [UpdateOperation]
        public void StartVerification(StartVerificationRequest request)
        {
            //ExecuteOperation(request.WorklistItem, new Operations.StartVerification());
        }

        [UpdateOperation]
        public void CompleteVerification(CompleteVerificationRequest request)
        {
            //ExecuteOperation(request.WorklistItem, new Operations.CompleteVerification());
        }

        #endregion
    }
}
