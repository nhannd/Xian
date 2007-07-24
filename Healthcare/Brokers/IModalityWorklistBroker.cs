using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Modality;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IModalityWorklistBroker : IPersistenceBroker
    {
        IList<WorklistItem> GetScheduledWorklist();
        IList<WorklistItem> GetScheduledWorklist(TechnologistScheduledWorklist worklist);
        IList<WorklistItem> GetCheckedInWorklist();
        IList<WorklistItem> GetCheckedInWorklist(TechnologistCheckedInWorklist worklist);
        IList<WorklistItem> GetInProgressWorklist();
        IList<WorklistItem> GetInProgressWorklist(TechnologistInProgressWorklist worklist);
        IList<WorklistItem> GetSuspendedWorklist();
        IList<WorklistItem> GetSuspendedWorklist(TechnologistSuspendedWorklist worklist);
        IList<WorklistItem> GetCompletedWorklist();
        IList<WorklistItem> GetCompletedWorklist(TechnologistCompletedWorklist worklist);
        IList<WorklistItem> GetCancelledWorklist();
        IList<WorklistItem> GetCancelledWorklist(TechnologistCancelledWorklist worklist);

        int GetScheduledWorklistCount();
        int GetScheduledWorklistCount(TechnologistScheduledWorklist worklist);
        int GetCheckedInWorklistCount();
        int GetCheckedInWorklistCount(TechnologistCheckedInWorklist worklist);
        int GetInProgressWorklistCount();
        int GetInProgressWorklistCount(TechnologistInProgressWorklist worklist);
        int GetSuspendedWorklistCount();
        int GetSuspendedWorklistCount(TechnologistSuspendedWorklist worklist);
        int GetCompletedWorklistCount();
        int GetCompletedWorklistCount(TechnologistCompletedWorklist worklist);
        int GetCancelledWorklistCount();
        int GetCancelledWorklistCount(TechnologistCancelledWorklist worklist);
    }
}
