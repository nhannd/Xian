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
    public class WorkQueueQueryParameters : ProcedureParameters
    {
        public WorkQueueQueryParameters()
            : base("QueryWorkQueue")
        { }

		public bool MemoryLimited
        {
			set { SubCriteria["MemoryLimited"] = new ProcedureParameter<bool>("MemoryLimited", value); }
        }

        public string ProcessorID
        {
            set { SubCriteria["ProcessorID"] = new ProcedureParameter<string>("ProcessorID", value); }
        }

		public WorkQueuePriorityEnum WorkQueuePriorityEnum
		{
			set { SubCriteria["WorkQueuePriorityEnum"] = new ProcedureParameter<ServerEnum>("WorkQueuePriorityEnum", value); }
		}

    }
}
