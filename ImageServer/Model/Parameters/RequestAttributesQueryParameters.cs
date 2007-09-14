using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class RequestAttributesQueryParameters : ProcedureParameters
    {
        public RequestAttributesQueryParameters()
            : base("QueryRequestAttributes")
        {
        }

        public ServerEntityKey SeriesKey
        {
            set { this.SubCriteria["SeriesKey"] = new ProcedureParameter<ServerEntityKey>("SeriesKey", value); }
        }
    }
}
