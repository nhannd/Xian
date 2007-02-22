using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Workflow.Modality;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IModalityWorklistBroker : IPersistenceBroker
    {
        IList<WorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria, string patientProfileAuthority);
        WorklistQueryResult GetWorklistItem(EntityRef<ModalityProcedureStep> mpsRef, string patientProfileAuthority);
    }
}
