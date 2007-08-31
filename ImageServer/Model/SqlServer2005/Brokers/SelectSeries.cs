using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Database.SqlServer2005;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.Brokers
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
