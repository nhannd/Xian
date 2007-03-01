using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Registration;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IRegistrationWorklistBroker : IPersistenceBroker
    {
        IList<WorklistQueryResult> GetWorklist(SearchCriteria criteria, PatientProfileSearchCriteria profileCriteria);
    }
}
