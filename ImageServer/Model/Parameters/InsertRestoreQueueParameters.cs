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
	public class InsertRestoreQueueParameters : ProcedureParameters
    {
        public InsertRestoreQueueParameters()
            : base("InsertRestoreQueue")
        {
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
		public ServerEntityKey ArchiveStudyStorageKey
		{
			set { SubCriteria["ArchiveStudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("ArchiveStudyStorageKey", value); }
		}
	}
}
