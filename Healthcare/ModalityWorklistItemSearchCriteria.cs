using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class ModalityWorklistItemSearchCriteria : WorklistItemSearchCriteria
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public ModalityWorklistItemSearchCriteria()
        {
        }

        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public ModalityWorklistItemSearchCriteria(string key)
            : base(key)
        {
        }

        public ClearCanvas.Healthcare.RequestedProcedureSearchCriteria RequestedProcedure
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("RequestedProcedure"))
                {
                    this.SubCriteria["RequestedProcedure"] = new ClearCanvas.Healthcare.RequestedProcedureSearchCriteria("RequestedProcedure");
                }
                return (ClearCanvas.Healthcare.RequestedProcedureSearchCriteria)this.SubCriteria["RequestedProcedure"];
            }
        }

        public ClearCanvas.Healthcare.ProcedureCheckInSearchCriteria ProcedureCheckIn
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ProcedureCheckIn"))
                {
                    this.SubCriteria["ProcedureCheckIn"] = new ClearCanvas.Healthcare.ProcedureCheckInSearchCriteria("ProcedureCheckIn");
                }
                return (ClearCanvas.Healthcare.ProcedureCheckInSearchCriteria)this.SubCriteria["ProcedureCheckIn"];
            }
        }

        public ClearCanvas.Healthcare.ProcedureStepSearchCriteria ProcedureStep
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ProcedureStep"))
                {
                    this.SubCriteria["ProcedureStep"] = new ClearCanvas.Healthcare.ModalityProcedureStepSearchCriteria("ProcedureStep");
                }
                return (ClearCanvas.Healthcare.ModalityProcedureStepSearchCriteria)this.SubCriteria["ProcedureStep"];
            }
        }
    }
}
