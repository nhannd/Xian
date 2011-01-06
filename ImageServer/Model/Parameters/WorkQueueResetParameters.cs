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
    public class WorkQueueResetParameters : ProcedureParameters
    {
        public WorkQueueResetParameters()
            : base("ResetWorkQueue") // name of the stored procedure
        { }

        public string ProcessorID
        {
            set { this.SubCriteria["ProcessorID"] = new ProcedureParameter<string>("ProcessorID", value); }
        }

        public int MaxFailureCount
        {
            set { this.SubCriteria["MaxFailureCount"] = new ProcedureParameter<int>("MaxFailureCount", value); }
        }

        public DateTime RescheduleTime
        {
            set { this.SubCriteria["RescheduleTime"] = new ProcedureParameter<DateTime>("RescheduleTime", value); }
        }
        public DateTime FailedExpirationTime
        {
            set { this.SubCriteria["FailedExpirationTime"] = new ProcedureParameter<DateTime>("FailedExpirationTime", value); }
        }

        public DateTime RetryExpirationTime
        {
            set { this.SubCriteria["RetryExpirationTime"] = new ProcedureParameter<DateTime>("RetryExpirationTime", value); }
        }
    
        
    }
}
