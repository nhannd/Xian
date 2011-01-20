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
    public class InsertStudyStorageParameters : ProcedureParameters
    {
        public InsertStudyStorageParameters()
            : base("InsertStudyStorage")
        {
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }
        public String StudyInstanceUid
        {
            set { SubCriteria["StudyInstanceUid"] = new ProcedureParameter<String>("StudyInstanceUid", value); }
        }
        public ServerEntityKey FilesystemKey
        {
            set { SubCriteria["FilesystemKey"] = new ProcedureParameter<ServerEntityKey>("FilesystemKey", value); }
        }
        public String Folder
        {
            set { SubCriteria["Folder"] = new ProcedureParameter<String>("Folder", value); }
        }
		public String TransferSyntaxUid
		{
			set { SubCriteria["TransferSyntaxUid"] = new ProcedureParameter<String>("TransferSyntaxUid", value); }
		}
		public StudyStatusEnum StudyStatusEnum
		{
			set { SubCriteria["StudyStatusEnum"] = new ProcedureParameter<ServerEnum>("StudyStatusEnum", value); }
		}
		public QueueStudyStateEnum QueueStudyStateEnum
		{
			set { SubCriteria["QueueStudyStateEnum"] = new ProcedureParameter<ServerEnum>("QueueStudyStateEnum", value); }
		}
	}
}
