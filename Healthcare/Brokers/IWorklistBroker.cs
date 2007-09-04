using System.Collections;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IWorklistBroker : IEntityBroker<Worklist, WorklistSearchCriteria>
    {
        IList FindAllRegistrationWorklists(User currentUser);
        IList FindAllTechnologistWorklists(User currentUser);
        IList FindAllReportingWorklists(User currentUser);
        Worklist FindWorklist(string name, string type);
        bool NameExistsForType(string name, string type);
    }
}
