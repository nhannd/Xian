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
    public class SetStudyRelatedInstanceCountParameters : ProcedureParameters
    {
        public SetStudyRelatedInstanceCountParameters(ServerEntityKey studyStorageKey)
            : base("SetStudyRelatedInstanceCount")
        {
            StudyStorageKey = studyStorageKey;
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        
        public int StudyRelatedInstanceCount
        {
            set { SubCriteria["StudyRelatedInstanceCount"] = new ProcedureParameter<int>("StudyRelatedInstanceCount", value); }
        }

        public int StudyRelatedSeriesCount
        {
            set { SubCriteria["StudyRelatedSeriesCount"] = new ProcedureParameter<int>("StudyRelatedSeriesCount", value); }
        }
    }
}