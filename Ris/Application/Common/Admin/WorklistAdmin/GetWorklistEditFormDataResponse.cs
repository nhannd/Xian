using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class GetWorklistEditFormDataResponse : DataContractBase
    {
        [DataMember]
        public List<RequestedProcedureTypeGroupSummary> RequestedProcedureTypeGroups;

        [DataMember]
        public List<UserSummary> Users;

        // TODO: Strongly typed
        [DataMember]
        public List<string> WorklistTypes;
    }
}