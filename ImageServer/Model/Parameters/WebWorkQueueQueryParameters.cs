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
    public class WebWorkQueueQueryParameters : ProcedureParameters
    {
        public WebWorkQueueQueryParameters()
            : base("WebQueryWorkQueue")
        {
			//Declare output parameters here
			SubCriteria["ResultCount"] = new ProcedureParameter<int>("ResultCount");
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public string PatientID
        {
            set { SubCriteria["PatientID"] = new ProcedureParameter<string>("PatientID", value); }
        }

        public string PatientsName
        {
			set { SubCriteria["PatientsName"] = new ProcedureParameter<string>("PatientsName", value); }
        }

        public string ProcessorID
        {
            set { SubCriteria["ProcessorID"] = new ProcedureParameter<string>("ProcessorID", value); }
        }

        public DateTime? ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime?>("ScheduledTime", value); }
        }

        public string Type
        {
            set { SubCriteria["Type"] = new ProcedureParameter<string>("Type", value); }
        }

        public string Status
        {
            set { SubCriteria["Status"] = new ProcedureParameter<string>("Status", value); }
        }

        public WorkQueuePriorityEnum Priority
        {
            set { SubCriteria["Priority"] = new ProcedureParameter<ServerEnum>("Priority", value); }
        }

        public bool CheckDataAccess
        {
            set { SubCriteria["CheckDataAccess"] = new ProcedureParameter<bool>("CheckDataAccess", value); }
        }

        public string UserAuthorityGroupGUIDs
        {
            set { SubCriteria["UserAuthorityGroupGUIDs"] = new ProcedureParameter<string>("UserAuthorityGroupGUIDs", value); }
        }

		public int StartIndex
        {
			set { SubCriteria["StartIndex"] = new ProcedureParameter<int>("StartIndex", value); }
	    }

		public int MaxRowCount
		{
			set { SubCriteria["MaxRowCount"] = new ProcedureParameter<int>("MaxRowCount", value); }
		}

		public int ResultCount
		{
			get { return (SubCriteria["ResultCount"] as ProcedureParameter<int>).Value; }
		}
		
    }
}
