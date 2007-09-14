using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Database.SqlServer2005;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.Brokers
{
    /// <summary>
    /// Broker implementation for <see cref="IQueryModalitiesInStudy"/>
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class QueryModalitiesInStudy : ProcedureQueryBroker<ModalitiesInStudyQueryParameters, Series>, IQueryModalitiesInStudy
    {
        public QueryModalitiesInStudy()
            : base("QueryModalitiesInStudy")
        {
        }
    }
}
