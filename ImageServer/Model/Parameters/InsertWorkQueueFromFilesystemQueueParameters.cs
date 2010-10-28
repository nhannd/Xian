#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class InsertWorkQueueFromFilesystemQueueParameters : ProcedureParameters
    {
        public InsertWorkQueueFromFilesystemQueueParameters()
            : base("InsertWorkQueueFromFilesystemQueue")
        { }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }

        public bool DeleteFilesystemQueue
        {
            set { SubCriteria["DeleteFilesystemQueue"] = new ProcedureParameter<bool>("DeleteFilesystemQueue", value); }
        }

        public FilesystemQueueTypeEnum FilesystemQueueTypeEnum
        {
            set { SubCriteria["FilesystemQueueTypeEnum"] = new ProcedureParameter<ServerEnum>("FilesystemQueueTypeEnum", value); }
        }

		public WorkQueueTypeEnum WorkQueueTypeEnum
        {
            set { SubCriteria["WorkQueueTypeEnum"] = new ProcedureParameter<ServerEnum>("WorkQueueTypeEnum", value); }
        }
		public XmlDocument Data
		{
			set { SubCriteria["Data"] = new ProcedureParameter<XmlDocument>("Data", value); }
		}
    }
}
