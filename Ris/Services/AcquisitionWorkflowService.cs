using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using Iesi.Collections;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Workflow.Acquisition;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class AcquisitionWorkflowService : WorkflowServiceBase, IAcquisitionWorkflowService
    {
        [ReadOperation]
        public IList<ModalityWorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria)
        {
            IModalityWorklistBroker broker = CurrentContext.GetBroker<IModalityWorklistBroker>();
            return broker.GetWorklist(criteria, "UHN");
        }

        [ReadOperation]
        public ModalityWorklistQueryResult GetWorklistItem(EntityRef<ModalityProcedureStep> mpsRef)
        {
            IModalityWorklistBroker broker = CurrentContext.GetBroker<IModalityWorklistBroker>();
            return broker.GetWorklistItem(mpsRef, "UHN");
        }

        [ReadOperation]
        public ModalityProcedureStep LoadWorklistItemPreview(ModalityWorklistQueryResult item)
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
        public void StartProcedureStep(EntityRef<ModalityProcedureStep> stepRef)
        {
            ExecuteOperation(LoadStep(stepRef), new Operations.StartModalityProcedureStep());
        }

        [UpdateOperation]
        public void CompleteProcedureStep(EntityRef<ModalityProcedureStep> stepRef)
        {
            ExecuteOperation(LoadStep(stepRef), new Operations.CompleteModalityProcedureStep());
        }

        [UpdateOperation]
        public void CancelProcedureStep(EntityRef<ModalityProcedureStep> stepRef)
        {
            ExecuteOperation(LoadStep(stepRef), new Operations.CancelModalityProcedureStep());
        }

        [UpdateOperation]
        public void ExecuteOperation(EntityRef<ModalityProcedureStep> stepRef, string operationClassName)
        {
            ExecuteOperation(LoadStep(stepRef), 
                new ClearCanvas.Healthcare.Workflow.Acquisition.WorkflowOperationExtensionPoint(), operationClassName);
        }

        [ReadOperation]
        public IDictionary<string, bool> GetOperationEnablement(EntityRef<ModalityProcedureStep> stepRef)
        {
            return GetOperationEnablement(LoadStep(stepRef),
                new ClearCanvas.Healthcare.Workflow.Acquisition.WorkflowOperationExtensionPoint());
        }

        private ModalityProcedureStep LoadStep(EntityRef<ModalityProcedureStep> stepRef)
        {
            IModalityProcedureStepBroker broker = this.CurrentContext.GetBroker<IModalityProcedureStepBroker>();
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }
    }
}
