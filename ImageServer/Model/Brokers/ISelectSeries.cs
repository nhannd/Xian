using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    /// <summary>
    /// Broker for dynamic queries against the <see cref="Series"/> table.
    /// </summary>
    public interface ISelectSeries : ISelectBroker<SeriesSelectCriteria, Series>
    {
    }
}
