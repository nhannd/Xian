using System;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class DeleteInstanceParameters : ProcedureParameters
    {
        public DeleteInstanceParameters()
            : base("DeleteInstanceParameters")
        {
        }

        public ServerEntityKey StudyStorageKey
        {
            set { this.SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        public String SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<String>("SeriesInstanceUid", value); }
        }

        public String SOPInstanceUid
        {
            set { SubCriteria["SOPInstanceUid"] = new ProcedureParameter<String>("SOPInstanceUid", value); }
        }
    }
}