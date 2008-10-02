using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class UpdateQueueStudyStateParameters : ProcedureParameters
    {
        public UpdateQueueStudyStateParameters()
            : base("UpdateQueueStudyStateParameters")
        {
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
    }
}
