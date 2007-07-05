using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class RequestedProcedureTypeGroupSummary : DataContractBase
    {
        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Category;
    }
}