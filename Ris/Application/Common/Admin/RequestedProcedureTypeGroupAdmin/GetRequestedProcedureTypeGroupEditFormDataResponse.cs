using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class GetRequestedProcedureTypeGroupEditFormDataResponse : DataContractBase
    {
        [DataMember]
        public List<RequestedProcedureTypeSummary> RequestedProcedureTypes;

        [DataMember]
        public List<EnumValueInfo> Categories;
    }
}