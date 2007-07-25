using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class WorklistAssembler
    {
        public WorklistSummary GetWorklistSummary(Worklist worklist)
        {
            return new WorklistSummary(worklist.GetRef(), worklist.DisplayName, worklist.Description, WorklistFactory.Instance.GetWorklistType(worklist));
        }
    }
}
