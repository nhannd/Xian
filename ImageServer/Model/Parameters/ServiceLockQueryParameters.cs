#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
