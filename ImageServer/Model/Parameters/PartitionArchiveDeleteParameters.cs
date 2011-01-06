#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class PartitionArchiveDeleteParameters : ProcedureParameters
    {
        public PartitionArchiveDeleteParameters()
            : base("PartitionArchiveDelete")
        {
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { this.SubCriteria["@ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }
        public bool DeleteStudies
        {
            set { this.SubCriteria["DeleteStudies"] = new ProcedureParameter<bool>("DeleteStudies", value); }
        }
    }
}
