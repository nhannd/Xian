using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolGroupDetail : DataContractBase
    {
        public ProtocolGroupDetail()
        {
            Codes = new List<ProtocolCodeDetail>();
            ReadingGroups = new List<RequestedProcedureTypeGroupSummary>();
        }

        public ProtocolGroupDetail(string name, string description, List<ProtocolCodeDetail> codes, List<RequestedProcedureTypeGroupSummary> readingGroups)
        {
            Name = name;
            Description = description;
            Codes = codes;
            ReadingGroups = readingGroups;
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public List<ProtocolCodeDetail> Codes;

        [DataMember]
        public List<RequestedProcedureTypeGroupSummary> ReadingGroups;
    }
}
