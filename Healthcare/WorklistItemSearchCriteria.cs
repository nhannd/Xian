using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class WorklistItemSearchCriteria : SearchCriteria
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public WorklistItemSearchCriteria()
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
    }
}
