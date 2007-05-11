using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IModalityWorklistBroker : IPersistenceBroker
    {
        IList<WorklistItem> GetScheduledWorklist();
        IList<WorklistItem> GetCheckedInWorklist();
        IList<WorklistItem> GetInProgressWorklist();
        IList<WorklistItem> GetSuspendedWorklist();
        IList<WorklistItem> GetCompletedWorklist();
        IList<WorklistItem> GetCancelledWorklist();

        int GetScheduledWorklistCount();
        int GetCheckedInWorklistCount();
        int GetInProgressWorklistCount();
        int GetSuspendedWorklistCount();
        int GetCompletedWorklistCount();
        int GetCancelledWorklistCount();
    }
}
