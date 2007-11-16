using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
    [DataContract]
    public class ListProtocolGroupsForProcedureRequest : DataContractBase
    {
        public ListProtocolGroupsForProcedureRequest(EntityRef procedureRef)
        {
            ProcedureRef = procedureRef;
        }

        [DataMember]
        public EntityRef ProcedureRef;
    }
}