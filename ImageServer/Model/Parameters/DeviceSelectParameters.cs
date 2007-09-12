using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class DeviceSelectParameters: ProcedureParameters
    {
        public DeviceSelectParameters()
            : base("SelectDevice")
        {
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { this.SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }
        public String AeTitle
        {
            set { this.SubCriteria["AeTitle"] = new ProcedureParameter<String>("AeTitle", value); }
        }
    }
}
