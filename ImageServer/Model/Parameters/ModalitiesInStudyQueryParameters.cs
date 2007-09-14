using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class ModalitiesInStudyQueryParameters: ProcedureParameters
    {
        public ModalitiesInStudyQueryParameters()
            : base("QueryModalitiesInStudy")
        {
        }

        public ServerEntityKey StudyKey
        {
            set { this.SubCriteria["StudyKey"] = new ProcedureParameter<ServerEntityKey>("StudyKey", value); }
        }
 
    }
}
