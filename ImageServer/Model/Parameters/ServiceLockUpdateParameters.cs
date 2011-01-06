#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class ServiceLockUpdateParameters : ProcedureParameters
    {
        public ServiceLockUpdateParameters()
            : base("UpdateServiceLock")
        { }

        public ServerEntityKey ServiceLockKey
        {
            set { this.SubCriteria["ServiceLockKey"] = new ProcedureParameter<ServerEntityKey>("ServiceLockKey", value); }
        }

        public string ProcessorId
        {
            set { this.SubCriteria["ProcessorId"] = new ProcedureParameter<string>("ProcessorId", value); }
        }

        public bool Lock
        {
            set { this.SubCriteria["Lock"] = new ProcedureParameter<bool>("Lock", value); }
        }

        public DateTime ScheduledTime
        {
            set { this.SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public bool Enabled
        {
            set { this.SubCriteria["Enabled"] = new ProcedureParameter<bool>("Enabled", value); }
        }
    }
}
