using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class DeviceInsertParameters : ProcedureParameters
    {
        public DeviceInsertParameters()
            : base("InsertDevice")
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
        public String Description
        {
            set { this.SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }
        public String IpAddress
        {
            set { this.SubCriteria["IpAddress"] = new ProcedureParameter<String>("IpAddress", value); }
        }
        public int Port
        {
            set { this.SubCriteria["Port"] = new ProcedureParameter<int>("Port", value); }
        }
        public bool Active
        {
            set { this.SubCriteria["Active"] = new ProcedureParameter<bool>("Active", value); }
        }
        public bool Dhcp
        {
            set { this.SubCriteria["Dhcp"] = new ProcedureParameter<bool>("Dhcp", value); }
        }
    }
}
