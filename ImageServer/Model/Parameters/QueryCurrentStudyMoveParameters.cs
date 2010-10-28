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
    public class QueryCurrentStudyMoveParameters : ProcedureParameters
    {
        public QueryCurrentStudyMoveParameters()
            : base("QueryCurrentStudyMoveCount")
        {
        }

        public ServerEntityKey DeviceKey
        {
            set { this.SubCriteria["DeviceKey"] = new ProcedureParameter<ServerEntityKey>("DeviceKey", value); }
        }

    }
}