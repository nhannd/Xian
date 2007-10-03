using ClearCanvas.Common;
using ClearCanvas.ImageServer.Database.SqlServer2005;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.SelectBrokers
{
    /// <summary>
    /// Broker implementation for <see cref="ISelectServerPartition"/>
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class SelectServerPartition : SelectBroker<ServerPartitionSelectCriteria, ServerPartition>, ISelectServerPartition
    {
        public SelectServerPartition()
            : base("ServerPartition")
        { }
    }
}
