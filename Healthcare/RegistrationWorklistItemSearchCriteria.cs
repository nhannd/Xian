using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class RegistrationWorklistItemSearchCriteria : WorklistItemSearchCriteria
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public RegistrationWorklistItemSearchCriteria()
        {
        }

        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public RegistrationWorklistItemSearchCriteria(string key)
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

        public ClearCanvas.Healthcare.ProcedureStepSearchCriteria ProtocolProcedureStep
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ProtocolProcedureStep"))
                {
                    this.SubCriteria["ProtocolProcedureStep"] = new ClearCanvas.Healthcare.ProcedureStepSearchCriteria("ProtocolProcedureStep");
                }
                return (ClearCanvas.Healthcare.ProcedureStepSearchCriteria)this.SubCriteria["ProtocolProcedureStep"];
            }
        }

        public ClearCanvas.Healthcare.ProtocolSearchCriteria Protocol
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Protocol"))
                {
                    this.SubCriteria["Protocol"] = new ClearCanvas.Healthcare.ProtocolSearchCriteria("Protocol");
                }
                return (ClearCanvas.Healthcare.ProtocolSearchCriteria)this.SubCriteria["Protocol"];
            }
        }
    }
}
