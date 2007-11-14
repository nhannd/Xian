using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class ServiceLockQueryParameters : ProcedureParameters
    {
        public ServiceLockQueryParameters()
            : base("QueryServiceLock")
        { }

        public ServiceLockTypeEnum ServiceLockTypeEnum
        {
            set { this.SubCriteria["ServiceLockTypeEnum"] = new ProcedureParameter<ServerEnum>("ServiceLockTypeEnum", value); }
        }

        public string ProcessorId
        {
            set { this.SubCriteria["ProcessorId"] = new ProcedureParameter<string>("ProcessorId", value); }
        }
    }
}
