using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class ProtocolProcedureStepSearchCriteria : ProcedureStepSearchCriteria
    {
        public ProtocolProcedureStepSearchCriteria()
        {
        }

        public ProtocolProcedureStepSearchCriteria(string key)
            : base(key)
        {
        }

        public ProtocolSearchCriteria Protocol
        {
            get
            {
                if(!this.SubCriteria.ContainsKey("Protocol"))
                {
                    this.SubCriteria["Protocol"] = new ProtocolSearchCriteria("Protocol");
                }
                return (ProtocolSearchCriteria)this.SubCriteria["Protocol"];
            }
        }
    }
}
