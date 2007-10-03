using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Model.SelectBrokers
{
    public interface ISelectWorkQueue : ISelectBroker<WorkQueueSelectCriteria,WorkQueue>
    {
    }
}
