using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class ModalitiesInStudySelectParameters: ProcedureParameters
    {
        public ModalitiesInStudySelectParameters()
            : base("SelectModalitiesInStudy")
        {
        }

        public ServerEntityKey StudyKey
        {
            set { this.SubCriteria["StudyKey"] = new ProcedureParameter<ServerEntityKey>("StudyKey", value); }
        }
 
    }
}
