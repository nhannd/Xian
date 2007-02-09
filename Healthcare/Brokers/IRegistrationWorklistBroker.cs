using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IRegistrationWorklistBroker : IPersistenceBroker
    {
        IList<RegistrationWorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria, string patientProfileAuthority);
        RegistrationWorklistQueryResult GetWorklistItem(EntityRef<ModalityProcedureStep> mpsRef, string patientProfileAuthority);
    }
}
