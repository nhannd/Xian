using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class RequestedProcedureTypeGroupDetail : DataContractBase
    {
        public RequestedProcedureTypeGroupDetail()
        {
            Foo = "Foo";
        }

        [DataMember]
        public string Foo;

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Category;

        [DataMember]
        public List<RequestedProcedureTypeSummary> RequestedProcedureTypes;
    }
}