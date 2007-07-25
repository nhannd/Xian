using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Database.SqlServer2005.Brokers
{
    public class GetServerPartitions : ProcedureEntityReadBroker<ServerPartition>, IGetServerPartitions
    {

    }
}
