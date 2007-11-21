using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class WorklistItemSearchCriteria : EntitySearchCriteria
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public WorklistItemSearchCriteria()
        {
        }

        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public WorklistItemSearchCriteria(string key)
            : base(key)
        {
        }

        public ClearCanvas.Healthcare.PatientProfileSearchCriteria PatientProfile
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientProfile"))
                {
                    this.SubCriteria["PatientProfile"] = new ClearCanvas.Healthcare.PatientProfileSearchCriteria("PatientProfile");
                }
                return (ClearCanvas.Healthcare.PatientProfileSearchCriteria)this.SubCriteria["PatientProfile"];
            }
        }

        public ClearCanvas.Healthcare.OrderSearchCriteria Order
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Order"))
                {
                    this.SubCriteria["Order"] = new ClearCanvas.Healthcare.OrderSearchCriteria("Order");
                }
                return (ClearCanvas.Healthcare.OrderSearchCriteria)this.SubCriteria["Order"];
            }
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
                    this.SubCriteria["ProcedureStep"] = new ClearCanvas.Healthcare.ProcedureStepSearchCriteria("ProcedureStep");
                }
                return (ClearCanvas.Healthcare.ProcedureStepSearchCriteria)this.SubCriteria["ProcedureStep"];
            }
        }
    }
}
