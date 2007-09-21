using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

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
