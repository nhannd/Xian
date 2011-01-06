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
	public class WebQueryStudyIntegrityQueueParameters : ProcedureParameters
    {
		public WebQueryStudyIntegrityQueueParameters()
            : base("WebQueryStudyIntegrityQueue")
        {
			//Declare output parameters here
			SubCriteria["ResultCount"] = new ProcedureParameter<int>("ResultCount");
        }

        public ServerEntityKey ServerPartitionKey
        {
            set { SubCriteria["ServerPartitionKey"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionKey", value); }
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

		public string Description
		{
			set { SubCriteria["Description"] = new ProcedureParameter<string>("Description", value); }
		}

        public string StudyData
        {
			set { SubCriteria["StudyData"] = new ProcedureParameter<string>("StudyData", value); }
        }

        public DateTime? InsertTime
        {
            set { SubCriteria["InsertTime"] = new ProcedureParameter<DateTime?>("InsertTime", value); }
        }

		public StudyIntegrityReasonEnum StudyIntegrityReasonEnum
        {
			set { SubCriteria["StudyIntegrityReasonEnum"] = new ProcedureParameter<ServerEnum>("StudyIntegrityReasonEnum", value); }
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
