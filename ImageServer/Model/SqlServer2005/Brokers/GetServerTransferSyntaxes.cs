using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Database.SqlServer2005;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class GetServerTransferSyntaxes : ProcedureReadBroker<ServerTransferSyntax>, IGetServerTransferSyntaxes
    {
        public GetServerTransferSyntaxes()
            : base("ReadServerTransferSyntaxes")
        { }
    }
}
