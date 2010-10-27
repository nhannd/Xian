#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    public class PostponeWorkQueueParameters : ProcedureParameters
    {
        public PostponeWorkQueueParameters()
            : base("PostponeWorkQueue")
        { }

        public ServerEntityKey WorkQueueKey
        {
            set { SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }

        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public DateTime ExpirationTime 
        {
            set { SubCriteria["ExpirationTime"] = new ProcedureParameter<DateTime>("ExpirationTime", value); }
        }

        public bool UpdateWorkQueue
        {
            set { SubCriteria["UpdateWorkQueue"] = new ProcedureParameter<bool>("UpdateWorkQueue", value); }
            
        }


        public string Reason
        {
            set { SubCriteria["Reason"] = new ProcedureParameter<string>("Reason", value); }
        }
    }
}