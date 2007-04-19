using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Workflow.Registration;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IRegistrationWorklistBroker : IPersistenceBroker
    {
        IList<WorklistItem> GetWorklist(string worklistClassName);
        IList<RequestedProcedure> GetRequestedProcedureForPatient(Patient patient, string worklistClassName);
    }
}
