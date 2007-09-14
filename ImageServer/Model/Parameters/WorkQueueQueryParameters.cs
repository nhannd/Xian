using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class WorkQueueQueryParameters : ProcedureParameters
    {
        public WorkQueueQueryParameters()
            : base("QueryWorkQueue")
        { }

        public TypeEnum TypeEnum
        {
            set { this.SubCriteria["TypeEnum"] = new ProcedureParameter<ServerEnum>("TypeEnum", value); }
        }
    }
}
