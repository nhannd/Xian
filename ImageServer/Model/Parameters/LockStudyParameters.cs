using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class LockStudyParameters: ProcedureParameters
    {
        public LockStudyParameters()
            : base("LockStudy")
        {
            // This is output from the stored procedure
            SubCriteria["Successful"] = new ProcedureParameter<bool>("Successful"); 
            
        }

        public ServerEntityKey StudyStorageKey
        {
            set { this.SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public bool Lock
        {
            set { this.SubCriteria["Lock"] = new ProcedureParameter<bool>("Lock", value); }
        }

        public bool Successful
        {
            get
            {
                return (this.SubCriteria["Successful"] as ProcedureParameter<bool>).Value;
            }
        }

    }
}
