using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using Iesi.Collections;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class AcquisitionWorkflowService : HealthcareServiceLayer, IAcquisitionWorkflowService
    {
        [ReadOperation]
        public IList<AcquisitionWorklistItem> GetWorklist(ScheduledProcedureStepSearchCriteria criteria)
        {
            IAcquisitionWorklistBroker broker = this.CurrentContext.GetBroker<IAcquisitionWorklistBroker>();
            return broker.GetWorklist(criteria);
        }

        [ReadOperation]
        public ScheduledProcedureStep LoadWorklistItemPreview(AcquisitionWorklistItem item)
        {
            IScheduledProcedureStepBroker spsBroker = this.CurrentContext.GetBroker<IScheduledProcedureStepBroker>();
            IRequestedProcedureBroker rpBroker = this.CurrentContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = this.CurrentContext.GetBroker<IOrderBroker>();
            IPatientBroker patientBroker = this.CurrentContext.GetBroker<IPatientBroker>();

            ScheduledProcedureStep sps = spsBroker.Load(item.WorkflowStep);

            // force a whole bunch of relationships to load... this could be optimized by using fetch joins
            spsBroker.LoadRequestedProcedureForScheduledProcedureStep(sps);
            //rpBroker.LoadOrderForRequestedProcedure(sps.RequestedProcedure);
            orderBroker.LoadOrderingFacilityForOrder(sps.RequestedProcedure.Order);

            // these calls should not be necessary, but seems there is a bug with NH 1.0.2 and 2nd-level cache?
            orderBroker.LoadDiagnosticServiceForOrder(sps.RequestedProcedure.Order);
            spsBroker.LoadTypeForScheduledProcedureStep(sps);
            rpBroker.LoadTypeForRequestedProcedure(sps.RequestedProcedure);

            
            patientBroker.LoadProfilesForPatient( sps.RequestedProcedure.Order.Patient );
            return sps;
        }

        [UpdateOperation]
        public void StartProcedureStep(EntityRef<ScheduledProcedureStep> stepRef)
        {
            ScheduledProcedureStep step = LoadStep(stepRef);
            step.Start();
        }

        [UpdateOperation]
        public void CompleteProcedureStep(EntityRef<ScheduledProcedureStep> stepRef)
        {
            ScheduledProcedureStep step = LoadStep(stepRef);
            step.Complete();
        }

        [UpdateOperation]
        public void CancelProcedureStep(EntityRef<ScheduledProcedureStep> stepRef)
        {
            ScheduledProcedureStep step = LoadStep(stepRef);
            step.Discontinue();
        }

        private ScheduledProcedureStep LoadStep(EntityRef<ScheduledProcedureStep> stepRef)
        {
            IScheduledProcedureStepBroker broker = this.CurrentContext.GetBroker<IScheduledProcedureStepBroker>();
            return broker.Load(stepRef, EntityLoadFlags.CheckVersion);
        }
    }
}
