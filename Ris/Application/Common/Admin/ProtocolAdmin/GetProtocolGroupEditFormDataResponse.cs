using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class GetProtocolGroupEditFormDataResponse : DataContractBase
    {
        public GetProtocolGroupEditFormDataResponse(List<ProtocolCodeDetail> protocolCodes, List<RequestedProcedureTypeGroupSummary> readingGroups)
        {
            ProtocolCodes = protocolCodes;
            ReadingGroups = readingGroups;
        }

        [DataMember]
        public List<ProtocolCodeDetail> ProtocolCodes;

        [DataMember]
        public List<RequestedProcedureTypeGroupSummary> ReadingGroups;
    }
}