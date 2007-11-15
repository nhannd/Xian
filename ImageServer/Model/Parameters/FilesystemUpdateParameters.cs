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
            set { this.SubCriteria["FileSystemKey"] = new ProcedureParameter<ServerEntityKey>("FileSystemKey", value); }
            get { return ((ProcedureParameter<ServerEntityKey>)this.SubCriteria["FileSystemKey"]).Value; }
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

        public Decimal HighWatermark
        {
            set { SubCriteria["HighWatermark"] = new ProcedureParameter<Decimal>("HighWatermark", value); }
        }

        public Decimal LowWatermark
        {
            set { SubCriteria["LowWatermark"] = new ProcedureParameter<Decimal>("LowWatermark", value); }
        }

        public Decimal PercentFull
        {
            set { SubCriteria["PercentFull"] = new ProcedureParameter<Decimal>("PercentFull", value); }
        }

    }
}
