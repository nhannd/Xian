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
            IModalityWorklistBroker broker = this.CurrentContext.GetBroker<IModalityWorklistBroker>();
            return broker.GetWorklist(criteria);
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

        private ModalityProcedureStep LoadStep(EntityRef<ModalityProcedureStep> stepRef)
        {
            IModalityProcedureStepBroker broker = this.CurrentContext.GetBroker<IModalityProcedureStepBroker>();
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }
    }
}
