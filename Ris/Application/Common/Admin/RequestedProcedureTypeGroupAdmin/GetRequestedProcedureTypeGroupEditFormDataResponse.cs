using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class GetRequestedProcedureTypeGroupEditFormDataResponse : DataContractBase
    {
        public GetRequestedProcedureTypeGroupEditFormDataResponse()
        {
            RequestedProcedureTypes = new List<RequestedProcedureTypeSummary>();
        }

        [DataMember]
        public List<RequestedProcedureTypeSummary> RequestedProcedureTypes;
    }
}