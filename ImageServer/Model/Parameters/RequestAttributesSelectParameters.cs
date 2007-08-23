using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class RequestAttributesSelectParameters : ProcedureParameters
    {
        public RequestAttributesSelectParameters()
            : base("SelectRequestAttributes")
        {
        }

        public ServerEntityKey SeriesKey
        {
            set { this.SubCriteria["SeriesKey"] = new ProcedureParameter<ServerEntityKey>("SeriesKey", value); }
        }
    }
}
