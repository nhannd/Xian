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
	public class UpdateArchiveQueueParameters : ProcedureParameters
	{
		public UpdateArchiveQueueParameters()
			: base("UpdateArchiveQueue")
        {
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public ServerEntityKey ArchiveQueueKey
        {
            set { SubCriteria["ArchiveQueueKey"] = new ProcedureParameter<ServerEntityKey>("ArchiveQueueKey", value); }
        }

        public ArchiveQueueStatusEnum ArchiveQueueStatusEnum
        {
            set { SubCriteria["ArchiveQueueStatusEnum"] = new ProcedureParameter<ServerEnum>("ArchiveQueueStatusEnum", value); }
        }

        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

		public string FailureDescription
		{
			set { SubCriteria["FailureDescription"] = new ProcedureParameter<string>("FailureDescription", value); }
		}
	}
}
