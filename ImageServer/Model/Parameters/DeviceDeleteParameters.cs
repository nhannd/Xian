using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    
    public class DeviceDeleteParameters : ProcedureParameters
    {
        public DeviceDeleteParameters()
            : base("DeleteDevice")
        {
        }

        public ServerEntityKey DeviceGUID
        {
            set { this.SubCriteria["DeviceGUID"] = new ProcedureParameter<ServerEntityKey>("DeviceGUID", value); }
        }
        
    }
}
