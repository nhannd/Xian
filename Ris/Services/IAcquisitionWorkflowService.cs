using System;
using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    public interface IAcquisitionWorkflowService : IHealthcareServiceLayer
    {
        IList<AcquisitionWorklistItem> GetWorklist(ScheduledProcedureStepSearchCriteria criteria);
        ScheduledProcedureStep LoadWorklistItemPreview(AcquisitionWorklistItem item);

        void StartProcedureStep(EntityRef<ScheduledProcedureStep> stepRef);
        void CompleteProcedureStep(EntityRef<ScheduledProcedureStep> stepRef);
        void CancelProcedureStep(EntityRef<ScheduledProcedureStep> stepRef);
    }
}
