using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class StudyStorageInsertParameters : ProcedureParameters
    {
        public StudyStorageInsertParameters()
            : base("InsertStudyStorage")
        {
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { this.SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }
        public String StudyInstanceUid
        {
            set { this.SubCriteria["StudyInstanceUid"] = new ProcedureParameter<String>("StudyInstanceUid", value); }
        }
        public ServerEntityKey FilesystemKey
        {
            set { this.SubCriteria["FilesystemKey"] = new ProcedureParameter<ServerEntityKey>("FilesystemKey", value); }
        }
        public String Folder
        {
            set { this.SubCriteria["Folder"] = new ProcedureParameter<String>("Folder", value); }
        }
    }
}
