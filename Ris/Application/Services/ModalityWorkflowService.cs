using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Common;
using Iesi.Collections;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ModalityWorkflowService : WorkflowServiceBase, IModalityWorkflowService
    {
        [ReadOperation]
        public IList<WorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria)
        {
            IModalityWorklistBroker broker = CurrentContext.GetBroker<IModalityWorklistBroker>();
            return broker.GetWorklist(criteria, "UHN");
        }

        [ReadOperation]
        public WorklistQueryResult GetWorklistItem(EntityRef mpsRef)
        {
            IModalityWorklistBroker broker = CurrentContext.GetBroker<IModalityWorklistBroker>();
            return broker.GetWorklistItem(mpsRef, "UHN");
        }

        [ReadOperation]
        public ModalityProcedureStep LoadWorklistItemPreview(WorklistQueryResult item)
        {
            IModalityProcedureStepBroker spsBroker = this.CurrentContext.GetBroker<IModalityProcedureStepBroker>();
            IRequestedProcedureBroker rpBroker = this.CurrentContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = this.CurrentContext.GetBroker<IOrderBroker>();
            IPatientBroker patientBroker = this.CurrentContext.GetBroker<IPatientBroker>();

            ModalityProcedureStep sps = spsBroker.Load(item.ProcedureStep);

            // force a whole bunch of relationships to load... this could be optimized by using fetch joins
            //spsBroker.LoadRequestedProcedureForModalityProcedureStep(sps);
            //rpBroker.LoadOrderForRequestedProcedure(sps.RequestedProcedure);
            orderBroker.LoadOrderingFacilityForOrder(sps.RequestedProcedure.Order);

            // ensure that these associations are loaded
            orderBroker.LoadDiagnosticServiceForOrder(sps.RequestedProcedure.Order);
            spsBroker.LoadTypeForModalityProcedureStep(sps);
            rpBroker.LoadTypeForRequestedProcedure(sps.RequestedProcedure);

            
            patientBroker.LoadProfilesForPatient( sps.RequestedProcedure.Order.Patient );
            return sps;
        }

        [UpdateOperation]
        public void ExecuteOperation(EntityRef stepRef, string operationClassName)
        {
            ExecuteOperation(LoadStep(stepRef), 
                new ClearCanvas.Healthcare.Workflow.Modality.WorkflowOperationExtensionPoint(), operationClassName);
        }

        [ReadOperation]
        public IDictionary<string, bool> GetOperationEnablement(EntityRef stepRef)
        {
            return GetOperationEnablement(LoadStep(stepRef),
                new ClearCanvas.Healthcare.Workflow.Modality.WorkflowOperationExtensionPoint());
        }

        private ModalityProcedureStep LoadStep(EntityRef stepRef)
        {
            IModalityProcedureStepBroker broker = this.CurrentContext.GetBroker<IModalityProcedureStepBroker>();
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }
    }
}
