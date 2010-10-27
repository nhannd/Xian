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
    public class LockStudyParameters: ProcedureParameters
    {
        public LockStudyParameters()
            : base("LockStudy")
        {
            // This is output from the stored procedure
            SubCriteria["Successful"] = new ProcedureParameter<bool>("Successful");
            SubCriteria["FailureReason"] = new ProcedureParameter<string>("FailureReason");  
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public bool WriteLock
        {
            set { SubCriteria["WriteLock"] = new ProcedureParameter<bool>("WriteLock", value); }
        }

		public bool ReadLock
		{
			set { SubCriteria["ReadLock"] = new ProcedureParameter<bool>("ReadLock", value); }
		}

		public QueueStudyStateEnum QueueStudyStateEnum
		{
			set { SubCriteria["QueueStudyStateEnum"] = new ProcedureParameter<ServerEnum>("QueueStudyStateEnum", value); }
		}

        public bool Successful
        {
            get
            {
                return (SubCriteria["Successful"] as ProcedureParameter<bool>).Value;
            }
        }

        public string FailureReason
        {
            get
            {
                return (SubCriteria["FailureReason"] as ProcedureParameter<string>).Value;
            }
        }
    }
}
