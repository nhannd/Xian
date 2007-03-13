using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.ModalityWorkflow;
using ClearCanvas.Common;
using Iesi.Collections;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Healthcare.Workflow.Modality;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ModalityWorkflowService : WorkflowServiceBase, IModalityWorkflowService
    {
        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            
            // TODO Enum value
            //criteria.State.EqualTo(request.ActivityStatus);

            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistQueryResult, ModalityWorklistItem, List<ModalityWorklistItem>>(
                    PersistenceContext.GetBroker<IModalityWorklistBroker>().GetWorklist(criteria, request.PatientProfileAuthority),
                    delegate(WorklistQueryResult queryResult)
                    {
                        return assembler.CreateWorklistItem(queryResult);
                    }));
        }

        [ReadOperation]
        public GetWorklistItemResponse GetWorklistItem(GetWorklistItemRequest request)
        {
            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            WorklistQueryResult result = PersistenceContext.GetBroker<IModalityWorklistBroker>().GetWorklistItem(request.MPSRef, request.PatientProfileAuthority);
            return new GetWorklistItemResponse(assembler.CreateWorklistItem(result));
        }

        [ReadOperation]
        public LoadWorklistItemPreviewResponse LoadWorklistItemPreview(LoadWorklistItemPreviewRequest request)
        {
            IModalityProcedureStepBroker spsBroker = PersistenceContext.GetBroker<IModalityProcedureStepBroker>();
            IRequestedProcedureBroker rpBroker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = PersistenceContext.GetBroker<IOrderBroker>();
            IPatientBroker patientBroker = PersistenceContext.GetBroker<IPatientBroker>();

            ModalityProcedureStep sps = spsBroker.Load(request.MPSRef);

            // force a whole bunch of relationships to load... this could be optimized by using fetch joins
            //spsBroker.LoadRequestedProcedureForModalityProcedureStep(sps);
            //rpBroker.LoadOrderForRequestedProcedure(sps.RequestedProcedure);
            orderBroker.LoadOrderingFacilityForOrder(sps.RequestedProcedure.Order);

            // ensure that these associations are loaded
            orderBroker.LoadDiagnosticServiceForOrder(sps.RequestedProcedure.Order);
            spsBroker.LoadTypeForModalityProcedureStep(sps);
            rpBroker.LoadTypeForRequestedProcedure(sps.RequestedProcedure);

            patientBroker.LoadProfilesForPatient( sps.RequestedProcedure.Order.Patient );

            ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            return new LoadWorklistItemPreviewResponse(assembler.CreateWorklistPreview(sps, request.PatientProfileAuthority));
        }

        [UpdateOperation]
        public void ExecuteOperation(ExecuteOperationRequest request)
        {
            ExecuteOperation(LoadStep(request.MPSRef), 
                new ClearCanvas.Healthcare.Workflow.Modality.WorkflowOperationExtensionPoint(), request.OperationClassName);
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(LoadStep(request.MPSRef),
                new ClearCanvas.Healthcare.Workflow.Modality.WorkflowOperationExtensionPoint()));
        }

        private ModalityProcedureStep LoadStep(EntityRef stepRef)
        {
            IModalityProcedureStepBroker broker = this.CurrentContext.GetBroker<IModalityProcedureStepBroker>();
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }
    }
}
