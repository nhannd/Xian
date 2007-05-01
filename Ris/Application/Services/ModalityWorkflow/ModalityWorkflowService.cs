using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IModalityWorkflowService))]
    public class ModalityWorkflowService : WorkflowServiceBase, IModalityWorkflowService
    {
        public ModalityWorkflowService()
        {
            _worklistExtPoint = new ClearCanvas.Healthcare.Workflow.Modality.WorklistExtensionPoint();
        }

        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            ModalityProcedureStepSearchCriteria criteria = assembler.CreateSearchCriteria(request.SearchCriteria);

            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, ModalityWorklistItem, List<ModalityWorklistItem>>(
                    PersistenceContext.GetBroker<IModalityWorklistBroker>().GetWorklist(criteria, request.PatientProfileAuthority),
                    delegate(WorklistItem queryResult)
                    {
                        return assembler.CreateModalityWorklistItem(queryResult);
                    }));
        }

        [ReadOperation]
        public GetWorklistItemResponse GetWorklistItem(GetWorklistItemRequest request)
        {
            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            WorklistItem result = PersistenceContext.GetBroker<IModalityWorklistBroker>().GetWorklistItem(request.ProcedureStepRef, request.PatientProfileAuthority);
            return new GetWorklistItemResponse(assembler.CreateModalityWorklistItem(result));
        }

        [ReadOperation]
        public LoadWorklistItemPreviewResponse LoadWorklistItemPreview(LoadWorklistItemPreviewRequest request)
        {
            IModalityProcedureStepBroker spsBroker = PersistenceContext.GetBroker<IModalityProcedureStepBroker>();
            IRequestedProcedureBroker rpBroker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = PersistenceContext.GetBroker<IOrderBroker>();
            IPatientBroker patientBroker = PersistenceContext.GetBroker<IPatientBroker>();

            ModalityProcedureStep sps = spsBroker.Load(request.ProcedureStepRef);
            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            return new LoadWorklistItemPreviewResponse(assembler.CreateWorklistPreview(sps, request.PatientProfileAuthority));
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            return new GetOperationEnablementResponse(GetOperationEnablement(assembler.CreateWorklistItem(request.WorklistItem)));
        }
    }
}
