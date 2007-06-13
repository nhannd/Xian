using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IReportingWorklistBroker : IPersistenceBroker
    {
        IList<WorklistItem> GetScheduledInterpretationWorklist();
        IList<WorklistItem> GetMyInterpretationWorklist(Staff performingStaff);
        IList<WorklistItem> GetMyTranscriptionWorklist(Staff performingStaff);
        IList<WorklistItem> GetMyVerificationWorklist(Staff performingStaff);
        IList<WorklistItem> GetMyVerifiedWorklist(Staff performingStaff);

        int GetScheduledInterpretationWorklistCount();
        int GetMyInterpretationWorklistCount(Staff performingStaff);
        int GetMyTranscriptionWorklistCount(Staff performingStaff);
        int GetMyVerificationWorklistCount(Staff performingStaff);
        int GetMyVerifiedWorklistCount(Staff performingStaff);

        IList<Report> GetPriorReport(Patient patient);
    }
}
