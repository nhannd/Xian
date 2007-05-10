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
        IList<WorklistItem> GetScheduledWorklist();
        IList<WorklistItem> GetCheckInWorklist();
        IList<WorklistItem> GetInProgressWorklist();
        IList<WorklistItem> GetCompletedWorklist();
        IList<WorklistItem> GetCancelledWorklist();

        int GetScheduledWorklistCount();
        int GetCheckInWorklistCount();
        int GetInProgressWorklistCount();
        int GetCompletedWorklistCount();
        int GetCancelledWorklistCount();

        IList<RequestedProcedure> GetRequestedProcedureForCheckIn(Patient patient);
        IList<RequestedProcedure> GetRequestedProcedureForPatientPreview(Patient patient);
    }
}
