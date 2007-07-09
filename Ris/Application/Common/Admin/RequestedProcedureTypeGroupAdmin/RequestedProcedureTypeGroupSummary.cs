using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class RequestedProcedureTypeGroupSummary : DataContractBase
    {
        public RequestedProcedureTypeGroupSummary(EntityRef entityRef, string name, string description, EnumValueInfo category)
        {
            EntityRef = entityRef;
            Name = name;
            Description = description;
            Category = category;
        }

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