using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class ResetStudyStorageParameters : ProcedureParameters
    {
        public ResetStudyStorageParameters()
            : base("ResetStudyStorage")
        {
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
    }
}