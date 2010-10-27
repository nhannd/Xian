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
    public class SetSeriesRelatedInstanceCountParameters : ProcedureParameters
    {
        public SetSeriesRelatedInstanceCountParameters(ServerEntityKey studyStorageKey, string seriesInstanceUid)
            : base("SetSeriesRelatedInstanceCount")
        {
            StudyStorageKey = studyStorageKey;
            SeriesInstanceUid = seriesInstanceUid;
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public string SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }

        public int SeriesRelatedInstanceCount
        {
            set { SubCriteria["SeriesRelatedInstanceCount"] = new ProcedureParameter<int>("SeriesRelatedInstanceCount", value); } 
        }
    }
}