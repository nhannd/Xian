using ClearCanvas.Common;
using ClearCanvas.ImageServer.Database.SqlServer2005;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.SelectBrokers
{
    /// <summary>
    /// Broker implementation for <see cref="ISelectWorkQueue"/>
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class SelectWorkQueue : SelectBroker<WorkQueueSelectCriteria, WorkQueue>, ISelectWorkQueue
    {
        public SelectWorkQueue()
            : base("WorkQueue")
        { }
    }
}
