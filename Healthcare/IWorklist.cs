using System.Collections;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public interface IWorklistItemKey
    {
    }

    public interface IWorklistItem
    {
        IWorklistItemKey Key { get; set; }
    }

    public interface IWorklist
    {
        IList GetWorklist(Staff currentUserStaff, IPersistenceContext context);
        int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context);
        string Name { get; set; }
    }
}