using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model
{
    public class StudyStorageInsertParameters : ProcedureParameters
    {
        public StudyStorageInsertParameters()
            : base("InsertStudyStorage")
        {
        }

        public ServerEntityKey ServerPartitionRef
        {
            set { this.SubCriteria["ServerPartitionRef"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionRef", value); }
        }
        public String StudyInstanceUid
        {
            set { this.SubCriteria["StudyInstanceUid"] = new ProcedureParameter<String>("StudyInstanceUid", value); }
        }
        public ServerEntityKey FilesystemRef
        {
            set { this.SubCriteria["FilesystemRef"] = new ProcedureParameter<ServerEntityKey>("FilesystemRef", value); }
        }
        public String Folder
        {
            set { this.SubCriteria["Folder"] = new ProcedureParameter<String>("Folder", value); }
        }
        public DateTime ExpirationTime
        {
            set { this.SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime>("ExpirationTime", value); }
        }
        public DateTime ScheduledTime
        {
            set { this.SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

    }
}
