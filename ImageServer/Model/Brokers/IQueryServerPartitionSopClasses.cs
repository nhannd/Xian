using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    public interface IQueryServerPartitionSopClasses : IProcedureQueryBroker<PartitionSopClassQueryParameters, PartitionSopClass>
    {
    }
}
