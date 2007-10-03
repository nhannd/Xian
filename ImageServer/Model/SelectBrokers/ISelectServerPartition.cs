using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Model.SelectBrokers
{
    public interface ISelectServerPartition : ISelectBroker<ServerPartitionSelectCriteria, ServerPartition>
    {
    }
}
