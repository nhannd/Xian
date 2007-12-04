using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class ServerPartitionUpdateParameters : ProcedureParameters
    {

        public ServerPartitionUpdateParameters()
            : base("UpdateServerPartition")
        {
        }

        public ServerEntityKey ServerPartitionGUID
        {
            set { this.SubCriteria["ServerPartitionGUID"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionGUID", value); }
        }
        public bool Enabled
        {
            set { this.SubCriteria["Enabled"] = new ProcedureParameter<bool>("Enabled", value); }
        }
        public String Description
        {
            set { this.SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }
        public String AeTitle
        {
            set { this.SubCriteria["AeTitle"] = new ProcedureParameter<String>("AeTitle", value); }
        }
        public int Port
        {
            set { this.SubCriteria["Port"] = new ProcedureParameter<int>("Port", value); }
        }
        public String PartitionFolder
        {
            set { this.SubCriteria["PartitionFolder"] = new ProcedureParameter<String>("PartitionFolder", value); }
        }
        public int DefaultRemotePort
        {
            set { this.SubCriteria["DefaultRemotePort"] = new ProcedureParameter<int>("DefaultRemotePort", value); }
        }
        public bool AcceptAnyDevice
        {
            set { this.SubCriteria["AcceptAnyDevice"] = new ProcedureParameter<bool>("AcceptAnyDevice", value); }
        }
        public bool AutoInsertDevice
        {
            set { this.SubCriteria["AutoInsertDevice"] = new ProcedureParameter<bool>("AutoInsertDevice", value); }
        }
    }
}
