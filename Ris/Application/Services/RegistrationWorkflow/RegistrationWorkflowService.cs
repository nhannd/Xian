using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class RegistrationWorkflowService : WorklistService, IRegistrationWorkflowService
    {
        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            return assembler.CreateGetWorklistResponse(GetWorklist(request.WorklistClassName, assembler.CreateSearchCriteria(request.SearchCriteria)));
        }

        [ReadOperation]
        public LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request)
        {
            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            RegistrationWorklistPreview preview = assembler.CreateWorklistPreview(request.WorklistItem.PatientProfileRef, this.PersistenceContext);

            List<WorklistQueryResult> listQueryResult = GetQueryResultForWorklistItem(request.WorklistItem.WorklistClassName, request.WorklistItem);
            foreach (WorklistQueryResult result in listQueryResult)
            {
                preview.RICs.Add(new RICSummary(result.RequestedProcedureName, 
                                                result.OrderingPractitioner,
                                                "N/A",
                                                result.ProcedureStepScheduledStartTime,
                                                "N/A"));
            }

            return new LoadWorklistPreviewResponse(preview);
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(request.ProcedureStepRef));
        }

        [UpdateOperation]
        public void ExecuteOperation(ExecuteOperationRequest request)
        {
            ExecuteOperation(request.ProcedureStepRef, request.OperationClassName);
        }

        [ReadOperation]
        public GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request)
        {
            IRequestedProcedureBroker rpBroker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = PersistenceContext.GetBroker<IOrderBroker>();

            List<WorklistQueryResult> listQueryResult = GetQueryResultForWorklistItem(request.WorklistClassName, new WorklistItem(request.WorklistClassName, request.PatientProfileRef));
            List<EntityRef> rpRefList = new List<EntityRef>();
            List<CheckInTableItem> checkInItemList = new List<CheckInTableItem>();
            foreach (WorklistQueryResult queryResult in listQueryResult)
            {
                if (rpRefList.Contains(queryResult.RequestedProcedure) == false)
                {
                    rpRefList.Add(queryResult.RequestedProcedure);

                    RequestedProcedure rp = rpBroker.Load(queryResult.RequestedProcedure);
                    rpBroker.LoadOrderForRequestedProcedure(rp);
                    rpBroker.LoadTypeForRequestedProcedure(rp);
                    orderBroker.LoadOrderingFacilityForOrder(rp.Order);
                    orderBroker.LoadOrderingPractitionerForOrder(rp.Order);

                    checkInItemList.Add(new CheckInTableItem(rp.GetRef(), rp.Type.Name, rp.Order.SchedulingRequestDateTime, rp.Order.OrderingFacility));
                }
            }

            return new GetDataForCheckInTableResponse(checkInItemList);
        }

        [UpdateOperation]
        public CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request)
        {
            foreach (EntityRef rpRef in request.RequestedProcedures)
            {
                CheckInProcedureStep cps = new CheckInProcedureStep(rpRef);
                cps.Start(request.Staff);
                cps.Complete(request.Staff);

                RequestedProcedure rp = PersistenceContext.GetBroker<IRequestedProcedureBroker>().Load(rpRef);
                rp.CheckInProcedureSteps.Add(cps);
                PersistenceContext.Lock(rp, DirtyState.Dirty);
            }

            return new CheckInProcedureResponse();
        }
    }
}
