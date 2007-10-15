using System;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    /// <summary>
    /// Input parameters for <see cref="IInsertFilesystem"/>.
    /// </summary>
    public class FilesystemInsertParameters : ProcedureParameters
    {
        public FilesystemInsertParameters()
            : base("InsertFilesystem")
        {
        }

        public FilesystemTierEnum TypeEnum
        {
            set { SubCriteria["FilesystemTierEnum"] = new ProcedureParameter<ServerEnum>("FilesystemTierEnum", value); }
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
    }
}
