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

        public ServerEntityKey ServerPartitionRef
        {
            set { this.SubCriteria["ServerPartitionRef"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionRef", value); }
        }
        public String StudyInstanceUid
        {
            set { this.SubCriteria["StudyInstanceUid"] = new ProcedureParameter<String>("StudyInstanceUid", value); }
        }
    }
}
