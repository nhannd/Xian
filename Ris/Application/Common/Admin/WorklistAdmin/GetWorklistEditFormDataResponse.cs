using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class GetWorklistEditFormDataResponse : DataContractBase
    {
        [DataMember]
        public List<RequestedProcedureTypeGroupSummary> RequestedProcedureTypeGroups;

        // TODO: Strongly typed
        [DataMember]
        public List<string> WorklistTypes;
    }
}