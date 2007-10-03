using ClearCanvas.Common;
using ClearCanvas.ImageServer.Database.SqlServer2005;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.SelectBrokers
{
    /// <summary>
    /// Broker implementation for <see cref="ISelectSeries"/>
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class SelectSeries : SelectBroker<SeriesSelectCriteria, Series>, ISelectSeries
    {
        public SelectSeries()
            : base("Series")
        { }
    }
}