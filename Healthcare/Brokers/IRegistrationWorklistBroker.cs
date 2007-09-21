using System.Collections.Generic;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Registration;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IRegistrationWorklistBroker : IPersistenceBroker
    {
        IList<WorklistItem> GetScheduledWorklist();
        IList<WorklistItem> GetScheduledWorklist(RegistrationScheduledWorklist worklist);
        IList<WorklistItem> GetCheckInWorklist();
        IList<WorklistItem> GetCheckInWorklist(RegistrationCheckedInWorklist worklist);
        IList<WorklistItem> GetInProgressWorklist();
        IList<WorklistItem> GetInProgressWorklist(RegistrationInProgressWorklist worklist);
        IList<WorklistItem> GetCompletedWorklist();
        IList<WorklistItem> GetCompletedWorklist(RegistrationCompletedWorklist worklist);
        IList<WorklistItem> GetCancelledWorklist();
        IList<WorklistItem> GetCancelledWorklist(RegistrationCancelledWorklist worklist);

        int GetScheduledWorklistCount();
        int GetScheduledWorklistCount(RegistrationScheduledWorklist worklist);
        int GetCheckInWorklistCount();
        int GetCheckInWorklistCount(RegistrationCheckedInWorklist worklist);
        int GetInProgressWorklistCount();
        int GetInProgressWorklistCount(RegistrationInProgressWorklist worklist);
        int GetCompletedWorklistCount();
        int GetCompletedWorklistCount(RegistrationCompletedWorklist worklist);
        int GetCancelledWorklistCount();
        int GetCancelledWorklistCount(RegistrationCancelledWorklist worklist);

        IList<Order> GetOrdersForCheckIn(Patient patient);
        int GetOrdersForCheckInCount(Patient patient);

        IList<Order> GetOrdersForCancel(Patient patient);
        int GetOrdersForCancelCount(Patient patient);

        IList<Order> GetOrdersForPatientPreview(Patient patient);

        IList<WorklistItem> Search(
            string mrnID,
            string mrnAssigningAuthority,
            string healthcardID,
            string familyName,
            string givenName);
    }
}
