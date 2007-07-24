using System.Collections;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IWorklistBroker : IEntityBroker<Worklist, WorklistSearchCriteria>
    {
        IList FindAllRegistrationWorklists(Staff currentStaff);
        IList FindAllTechnologistWorklists(Staff currentStaff);
        IList FindAllReportingWorklists(Staff currentStaff);
    }
}
