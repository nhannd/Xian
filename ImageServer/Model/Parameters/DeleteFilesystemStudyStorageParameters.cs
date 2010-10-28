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
	public class DeleteFilesystemStudyStorageParameters : ProcedureParameters
	{
		public DeleteFilesystemStudyStorageParameters()
			: base("DeleteFilesystemStudyStorage")
        {
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }
        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
		public StudyStatusEnum StudyStatusEnum
		{
			set { SubCriteria["StudyStatusEnum"] = new ProcedureParameter<ServerEnum>("StudyStatusEnum", value); }
		}
	}
}
