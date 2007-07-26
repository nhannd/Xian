using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Database.SqlServer2005.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class GetServerPartitions : ProcedureEntityReadBroker<ServerPartition>, IGetServerPartitions
    {
        public GetServerPartitions()
            : base("ReadServerPartitions")
        { }
    }
}
