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

        public ISearchCondition<Protocol> Protocol
        {
            get
            {
                if(!this.SubCriteria.ContainsKey("Protocol"))
                {
                    this.SubCriteria["Protocol"] = new SearchCondition<Protocol>("Protocol");
                }
                return (ISearchCondition<Protocol>) this.SubCriteria["Protocol"];
            }
        }
    }
}
