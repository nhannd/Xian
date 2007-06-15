using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IReportingWorklistBroker : IPersistenceBroker
    {
        IList<WorklistItem> GetToBeReportedWorklist();
        IList<WorklistItem> GetInProgressWorklist(Staff performingStaff);
        IList<WorklistItem> GetInTranscriptionWorklist(Staff performingStaff);
        IList<WorklistItem> GetToBeVerifiedWorklist(Staff performingStaff);
        IList<WorklistItem> GetVerifiedWorklist(Staff performingStaff);

        int GetToBeReportedWorklistCount();
        int GetInProgressWorklistCount(Staff performingStaff);
        int GetInTranscriptionWorklistCount(Staff performingStaff);
        int GetToBeVerifiedWorklistCount(Staff performingStaff);
        int GetVerifiedWorklistCount(Staff performingStaff);

        IList<Report> GetPriorReport(Patient patient);
    }
}
