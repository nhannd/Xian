using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class FilesystemUpdateParameters : ProcedureParameters
    {
        public FilesystemUpdateParameters()
            : base("UpdateFilesystem")
        {
        }

        public ServerEntityKey FileSystemKey
        {
            set { this.SubCriteria["GUID"] = new ProcedureParameter<ServerEntityKey>("GUID", value); }
            get { return ((ProcedureParameter<ServerEntityKey>)this.SubCriteria["GUID"]).Value; }
        }


        public String FilesystemPath
        {
            set { SubCriteria["FilesystemPath"] = new ProcedureParameter<String>("FilesystemPath", value); }
        }

        public String Description
        {
            set { SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }

        public bool Enabled
        {
            set { SubCriteria["Enabled"] = new ProcedureParameter<bool>("Enabled", value); }
        }

        public bool ReadOnly
        {
            set { SubCriteria["ReadOnly"] = new ProcedureParameter<bool>("ReadOnly", value); }
        }

        public bool WriteOnly
        {
            set { SubCriteria["WriteOnly"] = new ProcedureParameter<bool>("WriteOnly", value); }
        }

        public FilesystemTierEnum FilesystemTierEnum
        {
            set { this.SubCriteria["FilesystemTierEnum"] = new ProcedureParameter<ServerEnum>("FilesystemTierEnum", value); }
        }
    }
}
