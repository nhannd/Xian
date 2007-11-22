using ClearCanvas.Enterprise.Core;
using System;

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

        public ClearCanvas.Healthcare.ReportingProcedureStepSearchCriteria ReportingProcedureStep
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ReportingProcedureStep"))
                {
                    this.SubCriteria["ReportingProcedureStep"] = new ClearCanvas.Healthcare.ReportingProcedureStepSearchCriteria("ReportingProcedureStep");
                }
                return (ClearCanvas.Healthcare.ReportingProcedureStepSearchCriteria)this.SubCriteria["ReportingProcedureStep"];
            }
        }

        public ClearCanvas.Healthcare.ReportPartSearchCriteria ReportPart
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ReportPart"))
                {
                    this.SubCriteria["ReportPart"] = new ClearCanvas.Healthcare.ReportPartSearchCriteria("ReportPart");
                }
                return (ClearCanvas.Healthcare.ReportPartSearchCriteria)this.SubCriteria["ReportPart"];
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
