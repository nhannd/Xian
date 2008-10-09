using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class AttachStudyToPatientParamaters: ProcedureParameters
    {
        public AttachStudyToPatientParamaters()
            : base("AttachStudyToPatient")
        {
        }

        public ServerEntityKey StudyKey
        {
            set { SubCriteria["StudyKey"] = new ProcedureParameter<ServerEntityKey>("StudyKey", value); }
        }

        public ServerEntityKey NewPatientKey
        {
            set { SubCriteria["NewPatientKey"] = new ProcedureParameter<ServerEntityKey>("NewPatientKey", value); }
        }
    }
}
