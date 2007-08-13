using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model
{
    public class PartitionSopClassSelectParameters : ProcedureParameters
    {
        public PartitionSopClassSelectParameters()
            : base("SelectServerPartitionSopClasses")
        {
        }

        public ServerEntityKey ServerPartitionRef
        {
            set { this.SubCriteria["ServerPartitionRef"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionRef", value); }
        }
    }
}
