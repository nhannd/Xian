#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
    public class InsertWorkQueueParameters : ProcedureParameters
    {
        public InsertWorkQueueParameters()
            : base("InsertWorkQueue")
        { }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }
        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
		public WorkQueueTypeEnum WorkQueueTypeEnum
		{
			set { SubCriteria["WorkQueueTypeEnum"] = new ProcedureParameter<ServerEnum>("WorkQueueTypeEnum", value); }
		}
        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }
		public string SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }

        public string SopInstanceUid
        {
            set { SubCriteria["SopInstanceUid"] = new ProcedureParameter<string>("SopInstanceUid", value); }
        }

        public bool Duplicate
        {
            set { SubCriteria["Duplicate"] = new ProcedureParameter<bool>("Duplicate", value); }
            
        }
        public string Extension
        {
            set { SubCriteria["Extension"] = new ProcedureParameter<string>("Extension", value); }           
        }
		public ServerEntityKey StudyHistoryKey
		{
			set { SubCriteria["StudyHistoryKey"] = new ProcedureParameter<ServerEntityKey>("StudyHistoryKey", value); }
		}
		public ServerEntityKey DeviceKey
		{
			set { SubCriteria["DeviceKey"] = new ProcedureParameter<ServerEntityKey>("DeviceKey", value); }
		}
		public XmlDocument WorkQueueData
		{
			set
			{
				SubCriteria["Data"] = new ProcedureParameter<XmlDocument>("Data", value);
			}
		}

        public string WorkQueueGroupID
        {
            set { SubCriteria["WorkQueueGroupID"] = new ProcedureParameter<string>("WorkQueueGroupID", value); }
        }

        public string UidRelativePath
        {
            set { SubCriteria["UidRelativePath"] = new ProcedureParameter<string>("UidRelativePath", value); }
        }

        public string UidGroupID
        {
            set { SubCriteria["UidGroupID"] = new ProcedureParameter<string>("UidGroupID", value); }
        }
    }
}
