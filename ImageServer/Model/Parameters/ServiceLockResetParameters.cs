#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class ServiceLockResetParameters : ProcedureParameters
    {
        public ServiceLockResetParameters()
            : base("ResetServiceLock") // name of the stored procedure
        { }

        public string ProcessorId
        {
            set { this.SubCriteria["ProcessorId"] = new ProcedureParameter<string>("ProcessorId", value); }
        }
    }
}
