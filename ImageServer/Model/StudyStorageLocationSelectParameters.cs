using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model
{
    public class StudyStorageLocationSelectParameters : ProcedureParameters
    {
        public StudyStorageLocationSelectParameters()
            : base("SelectStudyStorageLocation")
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
    }
}
