using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class ReportingWorklistItemSearchCriteria : WorklistItemSearchCriteria
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public ReportingWorklistItemSearchCriteria()
        {
        }

        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public ReportingWorklistItemSearchCriteria(string key)
            : base(key)
        {
        }

        public ClearCanvas.Healthcare.ProcedureStepSearchCriteria ReportingProcedureStep
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ReportingProcedureStep"))
                {
                    this.SubCriteria["ReportingProcedureStep"] = new ClearCanvas.Healthcare.ProcedureStepSearchCriteria("ReportingProcedureStep");
                }
                return (ClearCanvas.Healthcare.ProcedureStepSearchCriteria)this.SubCriteria["ReportingProcedureStep"];
            }
        }
    }
}
