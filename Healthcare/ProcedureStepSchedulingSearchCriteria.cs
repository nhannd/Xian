using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    public class ProcedureStepSchedulingSearchCriteria : ActivitySchedulingSearchCriteria
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public ProcedureStepSchedulingSearchCriteria()
        {
        }

        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public ProcedureStepSchedulingSearchCriteria(string key)
            : base(key)
        {
        }

        public ProcedureStepPerformerSearchCriteria Performer
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Performer"))
                {
                    this.SubCriteria["Performer"] = new ProcedureStepPerformerSearchCriteria("Performer");
                }
                return (ProcedureStepPerformerSearchCriteria)this.SubCriteria["Performer"];
            }
        }
    }
}
