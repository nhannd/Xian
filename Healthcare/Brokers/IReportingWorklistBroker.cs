using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IReportingWorklistBroker : IPersistenceBroker
    {
        IList<WorklistItem> GetToBeReportedWorklist();
        IList<WorklistItem> GetToBeReportedWorklist(ReportingToBeReportedWorklist worklist);
        IList<WorklistItem> GetDraftWorklist(Staff performingStaff);
        IList<WorklistItem> GetInTranscriptionWorklist(Staff performingStaff);
        IList<WorklistItem> GetToBeVerifiedWorklist(Staff performingStaff);
        IList<WorklistItem> GetVerifiedWorklist(Staff performingStaff);

        int GetToBeReportedWorklistCount();
        int GetToBeReportedWorklistCount(ReportingToBeReportedWorklist worklist);
        int GetDraftWorklistCount(Staff performingStaff);
        int GetInTranscriptionWorklistCount(Staff performingStaff);
        int GetToBeVerifiedWorklistCount(Staff performingStaff);
        int GetVerifiedWorklistCount(Staff performingStaff);

        IList<Report> GetPriorReport(Patient patient);

        IList<WorklistItem> Search(
            string mrnID,
            string mrnAssigningAuthority,
            string healthcardID,
            string familyName,
            string givenName,
            string accessionNumber,
            bool showActiveOnly);
    }
}
