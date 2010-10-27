#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class RequestAttributesInsertParameters : ProcedureParameters
    {
        public RequestAttributesInsertParameters()
            : base("InsertRequestAttributes")
        {
        }

        public ServerEntityKey SeriesKey
        {
            set { this.SubCriteria["SeriesKey"] = new ProcedureParameter<ServerEntityKey>("SeriesKey", value); }
        }

        [DicomField(DicomTags.RequestedProcedureId, DefaultValue = DicomFieldDefault.Null)]
        public string RequestedProcedureId
        {
            set { this.SubCriteria["RequestedProcedureId"] = new ProcedureParameter<string>("RequestedProcedureId", value); }
        }

        [DicomField(DicomTags.ScheduledProcedureStepId, DefaultValue = DicomFieldDefault.Null)]
        public string ScheduledProcedureStepId
        {
            set { this.SubCriteria["ScheduledProcedureStepId"] = new ProcedureParameter<string>("ScheduledProcedureStepId", value); }
        }
    }
}
