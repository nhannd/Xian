using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Workflow.Registration;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IRegistrationWorklistBroker : IPersistenceBroker
    {
        IList<WorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria, PatientProfileSearchCriteria profileCriteria);
        WorklistQueryResult GetWorklistItem(EntityRef<ModalityProcedureStep> mpsRef, string patientProfileAuthority);
    }
}
