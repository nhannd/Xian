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
    public class WebResetWorkQueueParameters : ProcedureParameters
    {
        public WebResetWorkQueueParameters()
            : base("WebResetWorkQueueParameters")
        {
        }

        public ServerEntityKey WorkQueueKey
        {
            set { SubCriteria["WorkQueueKey"] = new ProcedureParameter<ServerEntityKey>("WorkQueueKey", value); }
        }

        public DateTime? NewScheduledTime
        {
            set { SubCriteria["NewScheduledTime"] = new ProcedureParameter<DateTime?>("NewScheduledTime", value); }
        }

        public DateTime? NewExpirationTime
        {
            set { SubCriteria["NewExpirationTime"] = new ProcedureParameter<DateTime?>("NewExpirationTime", value); }
        }

        public WorkQueuePriorityEnum NewPriority
        {
            set { SubCriteria["IssuerOfPatientId"] = new ProcedureParameter<ServerEnum>("NewPriority", value); }
        }

    }
}